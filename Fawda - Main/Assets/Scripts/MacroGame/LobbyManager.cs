using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    MinigameManager currentMinigame;

    public UnityEvent<bool> gameStartEvent;

    [SerializeField]
    ProfileData[] profiles = new ProfileData[5];
    List<short> gamepadPlayers;
    
    List<short> phonePlayers;
    
    void Start(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        phonePlayers = new List<short>();
        gamepadPlayers = new List<short>();
        ConnectionManager.singleton.RegisterRPC(OpCode.PROFILE_PAYLOAD, JoinPlayer);
    }

    [SerializeField]
    GameObject[] gameModePrefabs;
    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }


    void JoinPlayer(byte[] _data, int idx){
        print("Profile data " + _data.Length.ToString() +" bytes long - " + idx);
        profiles[idx] = new ProfileData(_data);
    }

    public void SetupMinigame(int _mode){
         
    }

}
