using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//The actual minigame logic
//Think of it like what a GM would look like in Mario Party
public abstract class DeployableMinigame
{
    public bool gameInPlay = false;
    public Dictionary<string, int> additionalConfig;
    public PlayerBehaviour[] playerInstances;
    public virtual void SetupGame(Transform _mapWrapper){
        DebugLogger.SourcedPrint("DeployableInstance (grandfather logic)", "Setup");

    }

    public virtual void EndGame(){

    }


}
