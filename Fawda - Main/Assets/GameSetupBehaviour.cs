using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSetupBehaviour
{
    private bool[] readies;

    public GameSetupBehaviour(){
        readies = new bool[5];
        DebugLogger.SourcedPrint("Game Setup (grandfather logic)", "Now accepting readies");
        ConnectionManager.singleton.RegisterRPC(OpCode.READYUP, ChangeReadyStatus);
    }

    public abstract void ReadyUp();

    protected abstract void OnReadyStatusChange(int _idx, bool _newVal);

    protected void ChangeReadyStatus(byte[] _data, int _idx){
        DebugLogger.SourcedPrint("Game Setup (grandfather logic)", "logic tripped");
        bool newVal = new PlayerGameReadyUpData(_data).ready;
        readies[_idx] = newVal;
        int cumCount = 0;
        foreach(bool r in readies) if (r) cumCount += 1;
        if (cumCount == LobbyManager.singleton.GetLobbySize()) ReadyUp();
        OnReadyStatusChange(_idx, newVal);
    }

    protected virtual void ResetReadies(){
        readies = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < readies.Length; i++){
            readies[i] = false;
        }
    }
}
