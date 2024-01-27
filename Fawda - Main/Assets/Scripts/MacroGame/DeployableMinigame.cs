using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class DeployableMinigame
{
    
    public abstract void SetupGame(int _specialityPlayer = -1);

    public virtual void EndGame(){


    }

    
}
