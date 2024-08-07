using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    public static GameManager gameManager;
    private InputManager inputManager;
    public static ProfileData[] players {get;private set;}
    Transform lobbyTransform;
    public UnityEvent<int> playerJoinEvent;
    public UnityEvent<int> playerRemoveEvent;

    void Awake(){
        if(singleton != null){
            DebugLogger.SourcedPrint("LobbyManager", "Singleton dispute, killing self", "FF0000");
            Destroy(this);
        }else{
            singleton = this;
        }
        gameManager = new GameManager(transform.Find("MapWrapper"));
        players = new ProfileData[5];
        playerJoinEvent = new UnityEvent<int>();
        playerRemoveEvent = new UnityEvent<int>();
        DebugLogger.SourcedPrint("LobbyManager", "Awake");
    }

    void Start(){
        inputManager = new InputManager();
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
    }



    void JoinPlayer(byte[] _data, int _idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + _idx);
        players[_idx] = new ProfileData(_data);
        print(players[_idx].name + " - " + _idx.ToString());
        playerJoinEvent.Invoke(_idx);
    }

    void RemovePlayer(int _idx){
        players[_idx] = null;
        playerRemoveEvent.Invoke(_idx);
    }

    public int GetLobbySize(){
        int size = 0;
        for(int i = 0; i < players.Length; i++){
            if(players[i] != null) size += 1;
        }
        return size;
    }

}
