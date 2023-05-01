using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LobbyManager : MonoBehaviour
{
    public void TogglePlayerControls(bool _engage){
        print("UDP FUCKING ENGAGE DAMNIT");
        ConnectionManager.singleton.SendMessageToClients(new NetMessage(OpCode.UDP_TOGGLE, BitConverter.GetBytes(_engage)));
    }
}
