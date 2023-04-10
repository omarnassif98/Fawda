using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
public class SynapseClient
{
    Thread clientThread;
    TcpClient client;
    bool connected;
    int idx;
    ConcurrentQueue<NetMessage> messageQueue = new ConcurrentQueue<NetMessage>();

    public void QueueMessage(NetMessage _netMessage){
        ClientConnection.singleton.PrintWrap("Message Queued");
        messageQueue.Enqueue(_netMessage);    
        ClientConnection.singleton.PrintWrap(String.Format("Queue size: {0}", messageQueue.Count));
    }
    
    void ConnectToServer(){
        
        while(connected){
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
                    int lengthOfIncoming = stream.ReadByte();
                    ClientConnection.singleton.PrintWrap(string.Format("{0} of with {1} bytes of data",Enum.GetName(typeof(OpCode),stream.ReadByte()),lengthOfIncoming));
                    int val = stream.ReadByte();
                    ClientConnection.singleton.PrintWrap(string.Format("val: {0} ", idx));
                }                
            }else if (!stream.CanRead){
                ClientConnection.singleton.PrintWrap("Cannot Read");
                client.Close();
            }else if (!stream.CanWrite){             
                ClientConnection.singleton.PrintWrap("Cannot Write");
                client.Close();
            }
        }
        ClientConnection.singleton.PrintWrap("Client exited");
    }


    
    public void kickoff(string _address)
    {
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
}
