using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;

    public UnityEvent<bool> gameStartEvent;

    [SerializeField]
    ProfileData[] profiles = new ProfileData[5];

    DeployableMinigame activeMinigame = null;
    
    void Start(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
    }

    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }


    void JoinPlayer(byte[] _data, int idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + idx);
        profiles[idx] = new ProfileData(_data);
    }

    public void IntroduceMinigame(string _mode){
        
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(minigameLogic);
        activeMinigame.SetupGame();
        }
}
