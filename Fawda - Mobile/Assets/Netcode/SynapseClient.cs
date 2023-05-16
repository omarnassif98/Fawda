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
                    if(code == OpCode.INDEX) idx = recievedBytes[0];
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

    public void FlashUDPMessage(NetMessage _msg){
        udpClient = new UdpClient();
        byte[] encodedMessage = SynapseMessageFormatter.EncodeMessage(_msg);
        byte[] signedMessage = new byte[encodedMessage.Length+1];
        Buffer.BlockCopy(encodedMessage,0,signedMessage,0,encodedMessage.Length);
        signedMessage[signedMessage.Length - 1] = (byte) idx;
        udpClient.Send(signedMessage, signedMessage.Length, addr, 10922);
    }


}
