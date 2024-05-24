using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class DebugSystemsManager : MonoBehaviour
{
    struct DebugPlayerInfo{
        public TMP_Text playerStatus;
        public TMP_Text playerInputHeartbeat;
        public RectTransform cursor;
        public DebugPlayerInfo(TMP_Text _playerStatus, TMP_Text _playerInputHeartbeat, RectTransform _cursor){
            playerStatus = _playerStatus;
            playerInputHeartbeat = _playerInputHeartbeat;
            cursor = _cursor;
        }
    }


    TMP_Text debugServerStatus, serverListen;
    private string debugServerStatusValue, serverListenValue;
    private GameObject debugBar, logger;
    private DebugPlayerInfo[] debugPlayerInfos;
    Queue<DateTime>[] heartBeats;


    void Awake(){
        debugBar = transform.Find("_DebugBar").gameObject;
        debugPlayerInfos = new DebugPlayerInfo[debugBar.transform.childCount];
        heartBeats = new Queue<DateTime>[debugBar.transform.childCount];
        for(int i = 0; i < debugBar.transform.childCount; i++){
            heartBeats[i] = new Queue<DateTime>();
            Transform holder = debugBar.transform.GetChild(i);
            debugPlayerInfos[i] = new DebugPlayerInfo(holder.Find("Prompt").GetComponent<TMP_Text>(), holder.Find("Heartbeat").GetComponent<TMP_Text>(), holder.Find("Joypad/Cursor").GetComponent<RectTransform>());
            SetDebug_Client_Status(i,false);
        }
        logger = transform.Find("_DebugLog").gameObject;
        serverListen = transform.Find("_DebugListen").GetComponent<TMP_Text>();
        debugServerStatus = transform.Find("_DebugServerStatus").GetComponent<TMP_Text>();

    }

    void Start(){
        LobbyManager.singleton.playerJoinEvent.AddListener((_idx) => SetDebug_Client_Status(_idx,true));
        LobbyManager.singleton.playerRemoveEvent.AddListener((_idx) => SetDebug_Client_Status(_idx,false));
    }

    public void SetDebug_Client_Status(int _idx, bool _connected){
        if(_connected) InputManager.heartbeatEvents[_idx].AddListener(() => AddHeartbeat(_idx));
        string status = _connected?"<color=#55FF00>Connected</color>":"<color=#FF0090>Disconnected</color>";
        debugPlayerInfos[_idx].playerStatus.text = string.Format("Client {0}: {1}", _idx + 1, status);
    }

    void AddHeartbeat(int _idx){
        heartBeats[_idx].Enqueue(DateTime.Now);
    }

    public void SetDebug_Server_Status(bool _status){
        string status = _status?"<color=#55FF00>Server Up</color>":"<color=#FF0090>Server Down</color>";
        debugServerStatusValue = status;
    }

    public void SetDebug_Listen_Status(bool _status){
        string status = _status?"<color=#55FF00>Listening</color>":"<color=#FF0090>Not Listening</color>";
        serverListenValue = status;
    }

    private void RefreshDebugInfo(){
        debugServerStatus.text = debugServerStatusValue;
        serverListen.text = serverListenValue;
        for(int i = 0; i<debugPlayerInfos.Length; i++) debugPlayerInfos[i].playerInputHeartbeat.text = heartBeats[i].Count.ToString() + "Hz";
    }
    void CleanHeartbeats(){
        DateTime cutoff = DateTime.Now.AddSeconds(-1);
        for(int i = 0; i < heartBeats.Length; i++){
            while(heartBeats[i].Count > 0 && heartBeats[i].Peek() < cutoff) heartBeats[i].Dequeue();
        }
    }
    void Update()
    {
        CleanHeartbeats();
        RefreshDebugInfo();
        if(Input.GetKeyDown(KeyCode.Tab)){
            debugBar.SetActive(!debugBar.activeInHierarchy);
            logger.SetActive(!logger.activeInHierarchy);
        }
        # if UNITY_EDITOR
            NetMessage msg = new NetMessage(OpCode.UDP_GAMEPAD_INPUT, new GamepadData(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"), Input.GetButtonDown("Action")).Encode());
            DirectedNetMessage dirMsg = new DirectedNetMessage(msg,0);
            ConnectionManager.singleton.QueueRPC(dirMsg);
        #endif
    }
}
