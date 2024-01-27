using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameManager : DeployableMinigame
{
   
    public HauntGameManager(){
        DebugLogger.singleton.Log("Booyah");

    }

    public override void SetupGame(int _specialityPlayer = -1)
    {
       DebugLogger.singleton.Log("HAUNT GAME CONFIGURED OH YEEEEEEEEAAH");
    }
}
