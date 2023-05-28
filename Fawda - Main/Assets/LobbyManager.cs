using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] gameModePrefabs;
    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }

    public void SetupMinigame(int _mode){
        GameObject minigame = GameObject.Instantiate(gameModePrefabs[_mode],Vector3.zero, Quaternion.identity, transform);
        minigame.GetComponent<MinigameManager>().SetupGame();
    }
}
