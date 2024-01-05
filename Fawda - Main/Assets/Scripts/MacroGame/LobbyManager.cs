using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    Transform gameSetupScreen;

    public UnityEvent<bool> gameStartEvent;

    [SerializeField]
    ProfileData[] players = new ProfileData[5];

    DeployableMinigame activeMinigame = null;

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
        UIManager.singleton.AddPlayerToRoster(players[idx].name, Color.cyan); //players[idx].colorSelection
        MenuCursorManager.singleton.UpdateCursorPlayer(players[idx], idx);
        playerJoinEvent.Invoke();
    }

    public void IntroduceMinigame(string _mode){
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(minigameLogic);
        gameSetupScreen.Find(_mode).gameObject.SetActive(true);
        UIManager.singleton.SetRoomCodeVisibility(false);
    }
}
