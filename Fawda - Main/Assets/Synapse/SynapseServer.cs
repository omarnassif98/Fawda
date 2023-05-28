using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System;

public class SynapseServer
{
    short clientQuantity = 5;
    TcpListener tcpServer;
    UdpClient udpServer;
    TcpClient[] tcpClients;
    Thread serverListenThread;
    Thread serverTickThread;
    Thread udpListenThread;
    public bool serverIsRunning, serverIsListening, udpEngaged;
    ConcurrentQueue<NetMessage>[] messageQueue;


    ////////
    //Unified
    ////////

    //Puts TCP Message in queue in seperate thread, the queue will be flushed on the main thread
    public void QueueMessageToClient(NetMessage _netMessage, short _idx = -1)
    {
        try
        {
            //Is directed to a client
            messageQueue[_idx].Enqueue(_netMessage);
        }
        catch (Exception)
        {
            //Is not directed to a client
            for (short i = 0; i < clientQuantity; i++)
            {
                messageQueue[i].Enqueue(_netMessage);
            }
        }
    }

    //Sets up the backend to be ready to receive connections
    private void InitializeBackend()
    {
        ConnectionManager.singleton.PrintWrap("Kicking Off");
        tcpClients = new TcpClient[clientQuantity];
        messageQueue = new ConcurrentQueue<NetMessage>[5];
        for (int i = 0; i < clientQuantity; i++)
        {
            tcpClients[i] = new TcpClient();
            messageQueue[i] = new ConcurrentQueue<NetMessage>();
        }
    }

    //Begins Listening, generates room code
    public void KickoffServer()
    {
        InitializeBackend();
        tcpServer = new TcpListener(IPAddress.Any, 23722);
        tcpServer.Start();
        udpServer = new UdpClient(10922);
        string room_code = GetRoomCode();
        ConnectionManager.singleton.SetRoomCode(room_code);
        serverIsListening = true;
        ConnectionManager.singleton.TriggerServerEvent("listen");
        ConnectionManager.singleton.PrintWrap("LISTENING");
        ThreadStart listenThreadStart = new ThreadStart(TcpListen);
        serverListenThread = new Thread(listenThreadStart);
        ThreadStart udpThreadStart = new ThreadStart(UdpListen);
        udpListenThread = new Thread(udpThreadStart);
        UIManager.debugSystems.SetDebug_Server_Status(true);
        serverListenThread.Start();
        udpListenThread.Start();
        ThreadStart tickThreadStart = new ThreadStart(Tick);
        serverTickThread = new Thread(tickThreadStart);
        serverTickThread.Start();
        serverIsRunning = true;
    }

    //A tick function, executes continuosly
    void Tick()
    {
        while (serverIsRunning)
        {
            for (int i = 0; i < tcpClients.Length; i++)
            {
                UIManager.debugSystems.SetDebug_Client_Status(i, tcpClients[i].Connected);
                if (!tcpClients[i].Connected) continue;
                try
                {
                    UpdateTcpStream(i);
                }
                catch (Exception e)
                {
                    ConnectionManager.singleton.PrintWrap("Client Force Quit");
                    ConnectionManager.singleton.PrintWrap(e.Message);
                    ConnectionManager.singleton.HandlePlayerDisconnect(i);
                }
            }
        }
    }

    //Kill Server, called when abrupt quit
    public void Kill()
    {
        serverIsListening = false;
        serverIsRunning = false;
        UIManager.debugSystems.SetDebug_Listen_Status(false);
        UIManager.debugSystems.SetDebug_Server_Status(false);
    }


    ///////////
    //TCP
    ///////////

    //Called by Tick Function, checks outbound message queue, sends them
    //Then reads and executes any pending messages.
    void UpdateTcpStream(int _idx)
    {
        NetworkStream stream = tcpClients[_idx].GetStream();
        if (stream.CanRead && stream.CanWrite)
        {

            if (messageQueue[_idx].Count > 0)
            {
                NetMessage msg;
                messageQueue[_idx].TryDequeue(out msg);
                byte[] outboundBytes = SynapseMessageFormatter.EncodeMessage(msg);
                stream.Write(outboundBytes, 0, outboundBytes.Length);
                ConnectionManager.singleton.PrintWrap(string.Format("Message sent to client {0}", _idx + 1));
            }

            if (stream.DataAvailable)
            {
                int size = stream.ReadByte();
                OpCode code = (OpCode)stream.ReadByte();
                byte[] recieveBytes = new byte[size];
                stream.Read(recieveBytes, 0, size);
                ConnectionManager.singleton.PrintWrap(string.Format("Client {0} - OPCODE: {1} - rawVal: {2}", _idx, Enum.GetName(typeof(OpCode), code), (short)recieveBytes[0]));
                NetMessage recreatedMsg =new NetMessage(code, recieveBytes);
                DirectedNetMessage finalizedMessage = new DirectedNetMessage(recreatedMsg, _idx);
                ConnectionManager.singleton.QueueRPC(finalizedMessage);
            }

        }
        else if (!stream.CanRead)
        {
            tcpClients[_idx].Close();
        }
        else if (!stream.CanWrite)
        {
            tcpClients[_idx].Close();
        }
    }

    //A listen thread, constantly listens and blocks until connection is made
    void TcpListen()
    {
        while (serverIsListening)
        {
            UIManager.debugSystems.SetDebug_Listen_Status(true);
            ConnectionManager.singleton.PrintWrap("Listening For New Client");
            TcpClient incomingConnection = tcpServer.AcceptTcpClient();
            ConnectionManager.singleton.PrintWrap("Connection Recieved");
            for (short i = 0; i < clientQuantity; i++)
            {
                if (tcpClients[i].Connected) continue;
                tcpClients[i] = incomingConnection;
                NetMessage initMessage = new NetMessage(OpCode.INDEX, new byte[] { (byte)i });
                QueueMessageToClient(initMessage, i);
                ConnectionManager.singleton.HandlePlayerConnect(i);
                break;
            }
        }
        UIManager.debugSystems.SetDebug_Listen_Status(false);
    }





    /////////
    //UDP
    /////////

    //UDP is connectionless, it listens to a single port for UDP messages from anyone and executes them
    void UdpListen(){
        ConnectionManager.singleton.PrintWrap("UDP IS UP!");
        while(serverIsRunning){
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 10922);
            byte[] receivedBytes = udpServer.Receive(ref endPoint);
            ConnectionManager.singleton.PrintWrap("Message Received");
            short length = receivedBytes[0];
            OpCode opCode = (OpCode)receivedBytes[1];
            short idx = receivedBytes[receivedBytes.Length - 1];
            ConnectionManager.singleton.PrintWrap(string.Format("RPC QUEUE: {0} of length {1} from player {2}", Enum.GetName(typeof(OpCode), opCode), length, idx));
            byte[] data = new byte[length];
            Buffer.BlockCopy(receivedBytes,2,data,0,length);
            NetMessage reconstruction = new NetMessage(opCode, data);
            DirectedNetMessage directedMsg = new DirectedNetMessage(reconstruction, idx);
            ConnectionManager.singleton.QueueRPC(directedMsg);
        }
    }


    ////////////
    //HELPER
    ////////////

    //Turns IP address into 5-7 character room code
     public string GetRoomCode()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        string candidate_ip = ip.Address.ToString();
                        short type = -1;
                        short stop_part = 1;
                        if (candidate_ip.StartsWith("10."))
                        {
                            type = 1;
                            stop_part = 0;
                        }


                        if (candidate_ip.StartsWith("172.16"))
                            type = 2;


                        if (candidate_ip.StartsWith("192.168"))
                            type = 3;


                        if (type == -1)
                            continue;
                        int[] code_format = Array.ConvertAll(candidate_ip.Split('.'), new Converter<string, int>(int.Parse));
                        string code = "";
                        code += type;
                        for (int i = 3; i > stop_part; i--)
                        {
                            code += code_format[i] > 0 ? code_format[i].ToString("X2") : "G";
                        }
                        return code;
                    }
                }
            }
        }
        return null;
    }

}