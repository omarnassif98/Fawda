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
        LobbyMenuManager.singleton.PoofLobby();
    }

    public virtual void StartGame() => LobbyManager.singleton.StartCoroutine(ShowTutorialIntro());

    public virtual void EndGame() => LobbyManager.singleton.StartCoroutine(WindDownGame());

    protected abstract IEnumerator ShowTutorialIntro();

    protected abstract IEnumerator WindDownGame();





}
