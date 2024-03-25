using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    private bool[] opt_ins;



    void Start(){
        InitializeLobbyConfigs();
        ConnectionManager.singleton.RegisterRPC("READYUP", IngestConfig);
    }

    private void IngestConfig(byte[] _data, int _idx){
        opt_ins[_idx] = BitConverter.ToBoolean(_data,1);
        base.ChangeReadyStatus(_idx, BitConverter.ToBoolean(_data,0));
    }

    public override void ReadyUp()
    {
        ConnectionManager.singleton.VacateRPC("READYUP");
        UIManager.RosterManager.rouletteDecisionEvent.AddListener(FinalizeSetup);
        UIManager.RosterManager.StartReadyupProcess(opt_ins);
    }

    private void FinalizeSetup(int _ghostIdx){
        UIManager.RosterManager.rouletteDecisionEvent.RemoveAllListeners();
        GameManager.singleton.ConfigureGame(_ghostIdx);
    }



    protected void InitializeLobbyConfigs(){
        base.ResetReadies();
        opt_ins = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++){
            opt_ins[i] = false;
        }

    }

    protected override void DeployMinigame()
    {
        throw new NotImplementedException();
    }
}
