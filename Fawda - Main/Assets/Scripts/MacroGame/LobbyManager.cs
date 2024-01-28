using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    Transform gameSetupScreen;
    
    ProfileData[] players = new ProfileData[5];

    public UnityEvent playerJoinEvent;
    
    void Start(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
        gameSetupScreen = GameObject.Find("Screens").transform.Find("Game Setup Screen");
    }

    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }


    void JoinPlayer(byte[] _data, int idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + idx);
        players[idx] = new ProfileData(_data);
        print(players[idx].name);
        UIManager.RosterManager.AddPlayerToRoster(players[idx].name, Color.cyan); //players[idx].colorSelection
        MenuCursorManager.singleton.UpdateCursorPlayer(players[idx], idx);
        playerJoinEvent.Invoke();
    }

    public void IntroduceMinigame(string _mode){
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        gameSetupScreen.Find(_mode).gameObject.SetActive(true);
        ConnectionManager.singleton.SendMessageToClients(OpCode.GAMESETUP, (int)Enum.Parse(typeof(GameCodes), _mode.ToUpper()));
        UIManager.singleton.SetRoomCodeVisibility(false);
    }

    public int GetLobbySize(){
        int size = 0;
        for(int i = 0; i < players.Length; i++){
            if(players[i] != null) size += 1;
        }
        return size;
    }

    public ProfileData[] GetPlayerProfiles() => players;

}
