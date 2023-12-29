using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    GameObject gameSetupScreen;

    public UnityEvent<bool> gameStartEvent;

    [SerializeField]
    ProfileData[] players = new ProfileData[5];

    DeployableMinigame activeMinigame = null;
    
    void Start(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
        gameSetupScreen = GameObject.Find("Game Setup Screen");
    }

    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }


    void JoinPlayer(byte[] _data, int idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + idx);
        players[idx] = new ProfileData(_data);
    }

    public void IntroduceMinigame(string _mode){
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(minigameLogic);
        gameSetupScreen.transform.Find(_mode).gameObject.SetActive(true);
        UIManager.singleton.SetRoomCodeVisibility(false);
    }
}
