using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
// FAWDA LAYER
// Object is basically a bridge between the game and the actual Synapse network
// Used for most all of communication that manifests as any change in the program these are delivered mostly by 
// - Emitted events for TCP
// - RPCs with data payloads for UDP
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
    private PlayerProfile[] playerProfiles = new PlayerProfile[5];
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

    public void RegisterServerEventListener(string _eventName, UnityAction _function){
        if(!serverEvents.ContainsKey(_eventName)) serverEvents[_eventName] = new UnityEvent();
        serverEvents[_eventName].AddListener(_function);
    }

    public void RegisterRPC(string _key, UnityAction<byte[], int> _func){
        PrintWrap("Registering " + _key);
        remoteProcCalls[_key] = _func;
    }

    public void QueueRPC(DirectedNetMessage _netMessage){
        rpcQueue.Enqueue(_netMessage);
    }


    /////
    // ACTUAL EXECUTION
    /////

    public void TriggerServerEvent(string _event){
        if(serverEvents.ContainsKey(_event)){
            serverEvents[_event].Invoke();
        }
    }

    private void FlushRPCQueue(){
        while(rpcQueue.Count > 0){
            DirectedNetMessage msg = rpcQueue.Dequeue();
            remoteProcCalls[Enum.GetName(typeof(OpCode), msg.msg.opCode)](msg.msg.val, msg.client);
        }
    }

    public void SetRoomCode(string _code){
        room_code = _code;
    }

    public void SendMessageToClients(NetMessage _msg){
        server.QueueMessageToClient(_msg);
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
        playerProfiles[_idx] = new PlayerProfile();
        live_players++;
        if(live_players == 1) queuedEvents.Enqueue("wakeup");
    }

    public void HandlePlayerDisconnect(int _idx){
        playerProfiles[_idx] = null;
        live_players--;
        if(live_players == 0) queuedEvents.Enqueue("sleep");
    }




}
