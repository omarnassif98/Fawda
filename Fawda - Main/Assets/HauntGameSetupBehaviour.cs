using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    List<int> readyOrder;

    public HauntGameSetupBehaviour() : base(){
        DebugLogger.SourcedPrint("GameSetup", "Now waiting for readies");
        readyOrder = new List<int>();
    }
    public override void ReadyUp()
    {
        DebugLogger.SourcedPrint("GameSetup", "All done");
    }

    protected override void OnReadyStatusChange(int _idx, bool _newVal)
    {
        DebugLogger.SourcedPrint("GameSetup", "logic trip");
        if(_newVal && !readyOrder.Contains(_idx)) readyOrder.Add(_idx);
        else if (!_newVal && readyOrder.Contains(_idx)) readyOrder.Remove(_idx);
        else DebugLogger.SourcedPrint("GameSetup", "Something seriously wrong ");
    }
}
