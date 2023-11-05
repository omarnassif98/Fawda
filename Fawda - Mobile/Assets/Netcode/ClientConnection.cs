using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections.Generic;

public class ClientConnection : MonoBehaviour
{
   public static ClientConnection singleton;
   SynapseClient client;
   [SerializeField] TMP_InputField code;
   [SerializeField] TMP_Text status;
   private Dictionary<string,UnityEvent> serverEvents;
   private Dictionary<string,UnityAction<byte[]>> remoteProcCalls  = new Dictionary<string, UnityAction<byte[]>>();
   private Queue<NetMessage> rpcQueue = new Queue<NetMessage>();
   private short playerIdx;

    void Start(){
        client = new SynapseClient();
        status.text = "Not Connected";
        RegisterRPC(OpCode.INDEX, SetPlayerIdx);
    }

    void Update(){
        FlushRPCQueue();
    }

    private void Awake(){
        if(singleton != null && singleton != this){
            Destroy(this);
        }else{
            singleton = this;
            serverEvents = new Dictionary<string, UnityEvent>();
            serverEvents["connect"] = new UnityEvent();
        }
    }

    private string ParseRoomCode(string code){
        string addr = "";
        try{
        switch (code[0])
        {
            case '1':
            addr += "10";
            break;
            case '2':
            addr += "172.16";
            break;
            case '3':
            addr += "192.168";
            break;
        }

        for(int i = code.Length - 1; i > 0; i--){
            addr += '.';
            if(code[i] == 'G'){
                addr += "0";
            }else{
                addr += int.Parse(code.Substring(i-1,2),System.Globalization.NumberStyles.HexNumber);
                i--;
            }
        }
        }catch (Exception ex){
            Debug.LogException(ex,this);
        }
        return addr;
    }

    public void Connect(){
        status.text = "Connecting";
        string addr = ParseRoomCode(code.text);
        client.kickoff(addr);
    }

    public void PrintWrap(string _message){
        print(string.Format("<color=#FFBF00>Synapse Client: </color>{0}",_message));
    }

    void SetPlayerIdx(byte[] _data){
        playerIdx = (short)_data[0];
    }
    //TCP
    public void SendMessageToServer(OpCode _opCode, byte[] _val){
        NetMessage msg = new NetMessage(_opCode, _val);
        client.QueueMessage(msg);
    }
    public void SendMessageToServer(OpCode _opCode, int _val){
        SendMessageToServer(_opCode, new byte[]{(byte)_val});
    }
    
    public void SendMessageToServer(OpCode _opCode){
        SendMessageToServer(_opCode, new byte[]{(byte)0});
    }

    public void FlashMessageToServer(OpCode _opCode, byte[] _val){
        NetMessage msg = new NetMessage(_opCode, _val);
        client.FlashUDPMessage(msg);
    }

    public void TriggerServerEvent(string _event){
        print("FIRE EVENT: " + _event);
        if(serverEvents.ContainsKey(_event)){
            serverEvents[_event].Invoke();
        }
    }

    public void OnDestroy(){
        client.Kill();
    }

    public void RegisterServerEventListener(string _eventName, UnityAction _function){
        print("REGISTER EVENT: " + _eventName);
        if(!serverEvents.ContainsKey(_eventName)) serverEvents[_eventName] = new UnityEvent();
        serverEvents[_eventName].AddListener(_function);
    }

    //Registers Listeners to RPCs
    public void RegisterRPC(string _key, UnityAction<byte[]> _func){
        PrintWrap("Registering " + _key);
        remoteProcCalls[_key] = _func;
    }

        public void RegisterRPC(OpCode _opCode, UnityAction<byte[]> _func){
        string opCode = Enum.GetName(typeof(OpCode), _opCode);
        PrintWrap("Registering " + opCode);
        remoteProcCalls[opCode] = _func;
    }

    //Flushes all RPCs, executing them all in FIFO order
    private void FlushRPCQueue(){
        while(rpcQueue.Count > 0){
            NetMessage msg = rpcQueue.Dequeue();
            PrintWrap(Enum.GetName(typeof(OpCode),msg.opCode));
            if(remoteProcCalls.ContainsKey(Enum.GetName(typeof(OpCode), msg.opCode))) remoteProcCalls[Enum.GetName(typeof(OpCode), msg.opCode)](msg.val);
        }
    }

    public void QueueRPC(NetMessage _netMessage){
        rpcQueue.Enqueue(_netMessage);
    }
    public short GetPlayerIdx(){
        return playerIdx;
    }

}
