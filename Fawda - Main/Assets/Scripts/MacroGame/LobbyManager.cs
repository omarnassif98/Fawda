using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager singleton;
    MinigameManager currentMinigame;

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
    }
    [SerializeField]
    GameObject[] gameModePrefabs;
    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }

    public void SetupMinigame(int _mode){
         

    }

}
