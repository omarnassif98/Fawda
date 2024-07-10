using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSetupBehaviour
{
    private bool[] readies;

    public GameSetupBehaviour(){
        readies = new bool[5];
        DebugLogger.SourcedPrint("GameSetup (grandfather logic)", "Now accepting readies");
    }

    public virtual void ReadyUp() => LobbyManager.gameManager.StartGame();


    protected void ChangeReadyStatus(byte[] _data, int _idx){
        DebugLogger.SourcedPrint("Game Setup (grandfather logic)", "logic tripped");
        bool newVal = new SimpleBooleanMessage(_data).ready;
        readies[_idx] = newVal;
        UIManager.RosterManager.SetPlayerRosterBadgeVisibility(_idx, newVal);
        int cumCount = 0;
        foreach(bool r in readies) if (r) cumCount += 1;
        if (cumCount == LobbyManager.singleton.GetLobbySize()) ReadyUp();
    }

    protected virtual void ResetReadies(){
        readies = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < readies.Length; i++){
            readies[i] = false;
        }
    }
}
