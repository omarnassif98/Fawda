using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class DeployableMinigame
{
    
    UnityEvent GameSetupEvent = new UnityEvent(), GameStartEvent = new UnityEvent(), GameEndEvent = new UnityEvent(), GamePauseEvent = new UnityEvent();
    public abstract void SetupGame();

    public virtual void EndGame(){
        GameEndEvent.Invoke();
    }

    public void AttachGameEndListener(UnityAction _callback){
        GameEndEvent.AddListener(_callback);
    }

}
