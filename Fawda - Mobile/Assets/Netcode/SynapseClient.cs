using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
public class SynapseClient
{
    Thread clientThread;
    TcpClient client;
    UdpClient udpClient;
    bool connected;
    int idx;
    string addr;
    ConcurrentQueue<NetMessage> messageQueue = new ConcurrentQueue<NetMessage>();


    ////////
    // Unified
    ////////
    public void QueueMessage(NetMessage _netMessage){
        ClientConnection.singleton.PrintWrap("Message Queued - " + Enum.GetName(typeof(OpCode), _netMessage.opCode));
        messageQueue.Enqueue(_netMessage);    
        ClientConnection.singleton.PrintWrap(String.Format("Queue size: {0}", messageQueue.Count));
    }
    
    void ConnectToServer(){
        while(connected){
            UpdateTCPStream();
        }
        ClientConnection.singleton.PrintWrap("Client exited");
    }

    public void kickoff(string _address)
    {
        addr = _address;
        client = new TcpClient(_address, 23722);
        ClientConnection.singleton.PrintWrap("Connected!");
        connected = true;
        ClientConnection.singleton.TriggerServerEvent("connect");
        ThreadStart kickoff = new ThreadStart(ConnectToServer);
        clientThread= new Thread(kickoff);
        clientThread.Start();        
    }

    public void Kill(){
        connected = false;
    }


    ///////
    // TCP
    ///////

    private void UpdateTCPStream(){
            NetworkStream stream = client.GetStream();
            if(stream.CanRead && stream.CanWrite){
                if(messageQueue.Count > 0){
                    ClientConnection.singleton.PrintWrap("Message Sent");
                    NetMessage msg;
                    messageQueue.TryDequeue(out msg);
                    byte[] sendBytes = SynapseMessageFormatter.EncodeMessage(msg);
                    stream.Write(sendBytes,0,sendBytes.Length);
                }
                if(stream.DataAvailable){
                    int size = stream.ReadByte();
                    OpCode code = (OpCode)stream.ReadByte();
                    byte[] recievedBytes = new byte[size];
                    stream.Read(recievedBytes, 0, size);
                    NetMessage msg = new NetMessage(code, recievedBytes);
                    ClientConnection.singleton.QueueRPC(msg);
                }                
            }else if (!stream.CanRead){
                ClientConnection.singleton.PrintWrap("Cannot Read");
                client.Close();
            }else if (!stream.CanWrite){             
                ClientConnection.singleton.PrintWrap("Cannot Write");
                client.Close();
            }
    }

    //////
    //UDP
    //////

    public void FlashUDPMessage(byte[] _data){
        udpClient = new UdpClient();
        NetMessage msg = new NetMessage(OpCode.UDP_INPUT, _data);
        byte[] udpBytes = SynapseMessageFormatter.EncodeMessage(msg);
        udpClient.Send(udpBytes, udpBytes.Length, addr, 10922);
    }


}
