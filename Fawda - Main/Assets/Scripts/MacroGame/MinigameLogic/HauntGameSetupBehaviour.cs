using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    [SerializeField] private bool[] opt_ins;



    void Start(){
        print("W EIS???");
        InitializeLobbyConfigs();
        ConnectionManager.singleton.RegisterRPC(OpCode.READYUP, IngestConfig);
        LobbyManager.singleton.playerJoinEvent.AddListener((idx) => InitializeLobbyConfigs());
        LobbyManager.singleton.playerRemoveEvent.AddListener((idx) => InitializeLobbyConfigs());
    }


    private void IngestConfig(byte[] _data, int _idx){
        opt_ins[_idx] = BitConverter.ToBoolean(_data,1);
        base.ChangeReadyStatus(_idx, BitConverter.ToBoolean(_data,0));
    }

    public override void ReadyUp()
    {
        ConnectionManager.singleton.VacateRPC("READYUP");
        UIManager.RosterManager.StartReadyupProcess(opt_ins);
    }

    protected void InitializeLobbyConfigs(){
        base.ResetReadies();
        int size = LobbyManager.singleton.GetLobbySize();
        opt_ins = new bool[size];
        for (int i = 0; i < size; i++){
            opt_ins[i] = false;
        }

    }

    protected override void DeployMinigame()
    {
        throw new NotImplementedException();
    }
}
