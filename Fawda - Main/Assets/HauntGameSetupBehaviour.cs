using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    private bool[] opt_ins;

    private bool[] readies;

    void Start(){
        opt_ins = new bool[LobbyManager.singleton.GetLobbySize()];
        readies = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++){
            opt_ins[i] = false;
            readies[i] = false;
        }
        ConnectionManager.singleton.RegisterRPC("READYUP", IngestConfig);
    }


    private void IngestConfig(byte[] _data, int _idx){
        readies[_idx] = BitConverter.ToBoolean(_data,0);
        opt_ins[_idx] = BitConverter.ToBoolean(_data,1);
    }
    public override void ReadyUp()
    {
        throw new System.NotImplementedException();
    }

}
