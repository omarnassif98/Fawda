using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class DeployableMinigame
{
    public bool gameInPlay = false;
    public Dictionary<string, int> additionalConfig;
    public abstract void SetupGame(Transform _mapWrapper, Dictionary<string, int> _additionalConfig = null);

    public virtual void EndGame(){

    }


}
