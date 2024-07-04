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
    short idx;
    string addr;
    ConcurrentQueue<NetMessage> messageQueue = new ConcurrentQueue<NetMessage>();


    ////////
    // Unified
    ////////
    public void QueueMessage(NetMessage _netMessage){
        DebugLogger.SourcedPrint("Synapse core","Message Queued - " + Enum.GetName(typeof(OpCode), _netMessage.opCode));
        messageQueue.Enqueue(_netMessage);
        ClientConnection.singleton.PrintWrap(String.Format("Queue size: {0}", messageQueue.Count));
    }

    void ConnectToServer(){
        while(connected){
            UpdateTCPStream();
        }
        DebugLogger.SourcedPrint("Synapse core","Client exited");
    }

    public void kickoff(string _address)
    {
        addr = _address;
        client = new TcpClient(_address, 23722);
        DebugLogger.SourcedPrint("Synapse core","Connected!");
        connected = true;
        ClientConnection.singleton.TriggerServerEvent("connect");
        ThreadStart kickoff = new ThreadStart(ConnectToServer);
        clientThread= new Thread(kickoff);
        clientThread.Start();
    }

    public void Kill(){
        DebugLogger.SourcedPrint("Synapse core","Disconected!");
        connected = false;
    }


    ///////
    // TCP
    ///////

    private void UpdateTCPStream(){
            NetworkStream stream = client.GetStream();
            if(stream.CanRead && stream.CanWrite){
                if(messageQueue.Count > 0){
                    DebugLogger.SourcedPrint("Synapse core","Message in Queue. Maybe clogged?");
                    NetMessage msg;
                    messageQueue.TryDequeue(out msg);
                    DebugLogger.SourcedPrint("Synapse core",string.Format("{0} Message Sending, not clogged", Enum.GetName(typeof(OpCode), msg.opCode)));
                    byte[] sendBytes = SynapseMessageFormatter.EncodeMessage(msg);
                    stream.Write(sendBytes,0,sendBytes.Length);
                }
                if(stream.DataAvailable){
                    int size = stream.ReadByte();
                    OpCode code = (OpCode)stream.ReadByte();
                    DebugLogger.SourcedPrint("Synapse core",string.Format("{0} Message Received", Enum.GetName(typeof(OpCode), code)));
                    byte[] recievedBytes = new byte[size];
                    stream.Read(recievedBytes, 0, size);
                    NetMessage msg = new NetMessage(code, recievedBytes);
                    if(code == OpCode.INDEX) idx = recievedBytes[0];
                    ClientConnection.singleton.QueueRPC(msg);
                }
            }else if (!stream.CanRead){
                DebugLogger.SourcedPrint("Synapse core","Cannot Read, closing connection");
                client.Close();
            }else if (!stream.CanWrite){
                DebugLogger.SourcedPrint("Synapse core","Cannot Write, closing connection");
                client.Close();
            }
    }

    //////
    //UDP
    //////

    public void FlashUDPMessage(NetMessage _msg){
        udpClient = new UdpClient();
        byte[] encodedMessage = SynapseMessageFormatter.EncodeMessage(_msg);
        byte[] signedMessage = new byte[encodedMessage.Length+1];
        Buffer.BlockCopy(encodedMessage,0,signedMessage,0,encodedMessage.Length);
        signedMessage[signedMessage.Length - 1] = (byte) idx;
        udpClient.Send(signedMessage, signedMessage.Length, addr, 10922);
    }


}
