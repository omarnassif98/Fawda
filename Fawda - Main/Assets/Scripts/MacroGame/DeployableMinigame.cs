using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class DeployableMinigame
{
    public bool gameInPlay = false;
    public abstract void SetupGame(Transform _mapWrapper, int _specialityPlayer = -1);

    public virtual void EndGame(){

    }


}
