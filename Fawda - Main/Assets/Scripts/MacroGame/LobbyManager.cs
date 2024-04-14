using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;

    //REWORK NEEDED
    Transform gameSetupScreen;

    public static ProfileData[] players {get;private set;}

    public UnityEvent<string, Color> playerJoinEvent;
    public UnityEvent<int> playerRemoveEvent;

    void Awake(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        players = new ProfileData[5];
        gameSetupScreen = GameObject.Find("Screens").transform.Find("Game Setup Screen");
        playerJoinEvent = new UnityEvent<string, Color>();
        playerRemoveEvent = new UnityEvent<int>();
    }

    void Start(){
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
    }

    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }


    void JoinPlayer(byte[] _data, int _idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + _idx);
        players[_idx] = new ProfileData(_data);
        print(players[_idx].name + " - " + _idx.ToString());
        MenuCursorManager.singleton.UpdateCursorPlayer(players[_idx], _idx);
        playerJoinEvent.Invoke(players[_idx].name, ResourceManager.namedColors[players[_idx].colorSelection].color);
    }

    void RemovePlayer(int _idx){
        players[_idx] = null;
        playerRemoveEvent.Invoke(_idx);
    }

    public void IntroduceMinigame(string _mode){
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        //REWORK NEEDED
        gameSetupScreen.Find(_mode).gameObject.SetActive(true);
        ConnectionManager.singleton.SendMessageToClients(OpCode.GAMESETUP, (int)Enum.Parse(typeof(GameCodes), _mode.ToUpper()));
        UIManager.singleton.SetRoomCodeVisibility(false);
    }

    public int GetLobbySize(){
        int size = 0;
        for(int i = 0; i < players.Length; i++){
            if(players[i] != null) size += 1;
        }
        print("LOBBY SIZE: " + size.ToString());
        return size;
    }

}
