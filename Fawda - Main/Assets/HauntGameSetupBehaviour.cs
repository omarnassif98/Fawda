using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : PromptedGameSetupBehaviour
{
    public override void ReadyUp()
    {
        DebugLogger.SourcedPrint("HauntGameSetup","All Ready");
        ConnectionManager.singleton.VacateRPC(OpCode.READYUP);
        base.ReadyUp();
    }
}
