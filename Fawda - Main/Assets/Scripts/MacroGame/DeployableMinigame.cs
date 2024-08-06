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
    protected Transform transform;

    public virtual void LoadMap() => LobbyMenuManager.singleton.PoofLobby();

    public abstract void SpawnPlayers();


    public virtual void ShowTutorialLoop() => LobbyManager.singleton.StartCoroutine(TutorialLoop());

    public virtual void EndGame() => LobbyManager.singleton.StartCoroutine(WindDownGame());



    protected void LocateMapTransform() => transform = LobbyManager.gameManager.mapWrapper;

    protected abstract IEnumerator TutorialLoop();

    protected abstract IEnumerator WindDownGame();





}
