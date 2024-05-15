using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Security.Permissions;
using System.Runtime.CompilerServices;

// FAWDA LAYER
// Object is basically a bridge between the game and the actual Synapse network
// Used for all of communication that manifests as any change in the program these are delivered mostly by
// - Emitted events
// - RPCs with data payloads
//   *these are both dicts of callable objs, queuedEvents / rpcQueue is for staging since they run on their own threads
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager singleton;
    SynapseServer server;
    private Dictionary<string,UnityEvent> serverEvents  = new Dictionary<string, UnityEvent>();
    private Dictionary<string,UnityAction<byte[], int>> remoteProcCalls  = new Dictionary<string, UnityAction<byte[], int>>();
    Queue<string> queuedEvents = new Queue<string>();
    private Queue<DirectedNetMessage> rpcQueue = new Queue<DirectedNetMessage>();
    private string room_code;
    private short live_players = 0;

    void Awake()
    {
        if (singleton != null && singleton != this){
            Destroy(this);
        }else{
            singleton = this;
        }
    }

    void Start(){
        // The Synapse component is a CHILD of this one. Not a singleton.
        server = new SynapseServer();
        server.KickoffServer();

    }

    void Update(){
        if(queuedEvents.Count > 0){
            TriggerServerEvent(queuedEvents.Dequeue());
        }

        FlushRPCQueue();
    }


    //////
    // STAGING
    //////

    // Registers events that are meant to simultaneosly affect many liteners - NOT BASED ON ANY OUTSIDE COMMUNICATION
    // The _func is literally a void
    // Triggering an event trips ALL (one to many) functions (actions) listening for that event
    public void RegisterServerEventListener(string _eventName, UnityAction _func, [CallerFilePath] string _caller = "", [CallerLineNumber] int _number = 0){
        string[] tmp = _caller.Split("/");
        string callerClass = tmp[tmp.Length-1].Split(".")[0];
        if(!serverEvents.ContainsKey(_eventName)){
            serverEvents[_eventName] = new UnityEvent();
            DebugLogger.SourcedPrint(string.Format("{0} via ConnectionManager",callerClass), "New Event " + _eventName, "FFAA00",_path:_caller, callingFileLineNumber:_number);
        }
        serverEvents[_eventName].AddListener(_func);
    }

    public void RegisterEphemeralServerEvent(string _eventName, UnityAction _func){
        UnityAction ephemeralAction = null;
        ephemeralAction = () => {_func(); VacateServerEventListener(_eventName, ephemeralAction);};
        RegisterServerEventListener(_eventName, ephemeralAction);
        DebugLogger.SourcedPrint("Connection Manager", "Registered ephemiral for " + _eventName);
    }
    public void VacateServerEventListener(string _eventName, UnityAction _func){
        serverEvents[_eventName].RemoveListener(_func);
        DebugLogger.SourcedPrint("Connection Manager", "Vacated single callback for " + _eventName, "FFAA00");
    }

    // What makes RPCs special is that you can pass parameters along from the client
    // The _func is literally a void

    public void RegisterRPC(OpCode _opCode, UnityAction<byte[], int> _func){
        string opCode = Enum.GetName(typeof(OpCode), _opCode);
        DebugLogger.SourcedPrint("Connection Manager","Registering RPC for " + Enum.GetName(typeof(OpCode),_opCode), "FFAA00");
        remoteProcCalls[opCode] = _func;
    }

    public void VacateRPC(OpCode _opCode){
        remoteProcCalls.Remove(Enum.GetName(typeof(OpCode), _opCode));
        DebugLogger.SourcedPrint("Connection Manager","REMOVING RPC - " + Enum.GetName(typeof(OpCode), _opCode), "FFAA00");
    }

    // Again called by the server - both TCP and UDP
    // Remember it runs on its own thread so RPCs can't be really remotely executed explicitly
    public void QueueRPC(DirectedNetMessage _netMessage){
        rpcQueue.Enqueue(_netMessage);
    }

    //////
    // COMMUNICATION
    /////


    /////
    // ACTUAL EXECUTION
    /////


    public void TriggerServerEvent(string _event){
        if(serverEvents.ContainsKey(_event)){
            serverEvents[_event].Invoke();
        }
    }

    public void FlushRPCQueue(){
        while(rpcQueue.Count > 0){
            DirectedNetMessage msg = rpcQueue.Dequeue();
            string code = Enum.GetName(typeof(OpCode), msg.msg.opCode);
            if(!remoteProcCalls.ContainsKey(code)) {DebugLogger.SourcedPrint("Synapse Client", "Got DOA RPC - " + code, "FFAA00"); continue;}
            remoteProcCalls[code](msg.msg.val, msg.client);
        }
    }

    public void SetRoomCode(string _code){
        room_code = _code;
    }

    public void SendMessageToClients(NetMessage _msg, int _idx = -1){
        server.QueueMessageToClient(_msg, _idx);
    }

    public void SendMessageToClients(OpCode _opCode, int _idx = -1){
        NetMessage msg = new NetMessage(_opCode, new byte[1]{0});
        server.QueueMessageToClient(msg, _idx);
    }

    public void SendMessageToClients(OpCode _opCode, byte[] _data, int _idx = -1){
        NetMessage msg = new NetMessage(_opCode, _data);
        server.QueueMessageToClient(msg, _idx);
    }

    public string GetRoomCode(){
        return room_code;
    }

    public void PrintWrap(string _msg){
        Debug.Log(string.Format("<color=#FFBF00>Synapse Client: </color>{0}",_msg));
    }

    public void OnDestroy(){
        server.Kill();
        PrintWrap("Quit");
    }

    public void HandlePlayerConnect(int _idx){
        PrintWrap("Connection");
        live_players++;
        if(live_players == 1) queuedEvents.Enqueue("wakeup");
    }

    public void HandlePlayerDisconnect(int _idx){
        live_players--;
        if(live_players == 0) queuedEvents.Enqueue("sleep");
    }


    /////
    // Information
    /////

    //Pulls in the CONNECTION idx of each player
    public short[] GetPlayerIdxs(){
        print("GOTTA FIX");
        return null;
    }

}
