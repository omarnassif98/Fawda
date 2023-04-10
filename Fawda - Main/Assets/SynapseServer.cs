using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System;

public class SynapseServer
{
    short clientQuantity = 5;
    TcpListener server;
    TcpClient[] clients;
    Thread serverListenThread;
    Thread serverTickThread;
    
    public bool serverIsRunning, serverIsListening;
    ConcurrentQueue<NetMessage>[] messageQueue;

    public void QueueMessageToClient(NetMessage _netMessage, short _idx = -1){
        try{
            messageQueue[_idx].Enqueue(_netMessage);
        }catch(Exception){
            for(short i = 0; i < clientQuantity; i++){
                messageQueue[_idx].Enqueue(_netMessage);        
            }
        }
    }

    void UpdateStream(int _idx){
        NetworkStream stream = clients[_idx].GetStream();
        if(stream.CanRead && stream.CanWrite){
            
            if(messageQueue[_idx].Count > 0){
                NetMessage msg;
                messageQueue[_idx].TryDequeue(out msg);
                byte[] outboundBytes = SynapseMessageFormatter.EncodeMessage(msg);
                stream.Write(outboundBytes,0,outboundBytes.Length);
                ConnectionManager.singleton.PrintWrap(string.Format("Message sent to client {0}",_idx + 1));
            }

            if(stream.DataAvailable){
                byte[] recieveBytes = new byte[clients[_idx].ReceiveBufferSize];
                stream.Read(recieveBytes, 0, (int)clients[_idx].ReceiveBufferSize);
                ConnectionManager.singleton.PrintWrap(string.Format("Client {0} did something (Decoding is a WIP)",_idx));
            }

        }else if (!stream.CanRead){
            clients[_idx].Close();
        }else if (!stream.CanWrite){             
            clients[_idx].Close();
        }
    }

    void Listen(){
        while(serverIsListening){
            UIManager.singleton.SetDebug_Listen_Status(true);
            ConnectionManager.singleton.PrintWrap("Listening For New Client");        
            TcpClient incomingConnection = server.AcceptTcpClient();
            ConnectionManager.singleton.PrintWrap("Connection Recieved");
            for(short i = 0; i < clientQuantity; i++){
                if(clients[i].Connected) continue;
                clients[i] = incomingConnection;
                NetMessage initMessage = new NetMessage(OpCode.INDEX,new byte[]{(byte)i});
                QueueMessageToClient(initMessage,i);
                break;
            }
        }
        UIManager.singleton.SetDebug_Listen_Status(false);
    } 

    void Tick(){
        while(serverIsRunning){
            for(int i = 0; i < clients.Length; i++){
                UIManager.singleton.SetDebug_Client_Status(i,clients[i].Connected);
                if(!clients[i].Connected){
                    continue;
                }

                try{
                    UpdateStream(i);
                }catch(Exception e){
                    ConnectionManager.singleton.PrintWrap("Client Force Quit");
                    ConnectionManager.singleton.PrintWrap(e.Message);
                }
            }
        }
    }


    private void InitializeBackend(){
        ConnectionManager.singleton.PrintWrap("Kicking Off");
        clients = new TcpClient[clientQuantity];
        messageQueue = new ConcurrentQueue<NetMessage>[5];
        for(int i = 0; i < clientQuantity; i++){
            clients[i] = new TcpClient();
            messageQueue[i] = new ConcurrentQueue<NetMessage>();
        }
    }

    public void KickoffServer(){
        InitializeBackend();
        server = new TcpListener(IPAddress.Any, 23722);
        server.Start();
        ConnectionManager.singleton.PrintWrap("LISTENING");
        ThreadStart listenThreadStart = new ThreadStart(Listen);
        serverListenThread = new Thread(listenThreadStart);
        UIManager.singleton.SetDebug_Server_Status(true);
        serverListenThread.Start();
        serverIsListening = true;
        ThreadStart tickThreadStart = new ThreadStart(Tick);
        serverTickThread = new Thread(tickThreadStart);
        serverTickThread.Start();
        serverIsRunning = true;
    }

    public void Kill(){
        serverIsListening = false;
        serverIsRunning = false;
        UIManager.singleton.SetDebug_Listen_Status(false);
        UIManager.singleton.SetDebug_Server_Status(false);
    }
}