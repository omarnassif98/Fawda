using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSetupBehaviour : MonoBehaviour
{
    [SerializeField]
    GameCodes gameCode;

    private bool[] readies;

    public abstract void ReadyUp();

    protected abstract void DeployMinigame();

    public virtual void OnEnable(){
        GameManager.singleton.LoadMinigame(gameCode);
    }

    public virtual void OnDisable(){
        GameManager.singleton.KillMinigame();
    }

    protected void ChangeReadyStatus(int _idx, bool _flag){
        readies[_idx] = _flag;
        int cumCount = 0;
        foreach(bool r in readies) if (r) cumCount += 1;
        if (cumCount == LobbyManager.singleton.GetLobbySize()) ReadyUp();
    }

    protected void ResetReadies(){
        readies = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < readies.Length; i++){
            readies[i] = false;
        }
    }
}
