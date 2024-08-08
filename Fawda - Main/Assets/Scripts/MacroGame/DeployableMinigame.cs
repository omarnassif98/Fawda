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
    protected IEnumerator tutorialEvent;

    public virtual void LoadMap() => LobbyMenuManager.singleton.PoofLobby();

    public abstract void SpawnPlayers();


    public virtual void ShowTutorialLoop()
    {
        tutorialEvent = TutorialLoop();
        LobbyManager.singleton.StartCoroutine(tutorialEvent);
    }


    public virtual void ClearPlayers()
    {
        foreach (PlayerBehaviour player in playerInstances) player.Terminate();
        playerInstances = null;
    }

   public void BeginGame()
    {
        ClearPlayers();
        SpawnPlayers();

    }


    public virtual void EndGame() => LobbyManager.singleton.StartCoroutine(WindDownGame());



    protected void LocateMapTransform() => transform = LobbyManager.gameManager.mapWrapper;

    protected abstract IEnumerator TutorialLoop();


    private IEnumerator CountGameIn()
    {

        yield return new WaitForSeconds(0.8f);

        yield return new WaitForSeconds(0.5f);

    }

    protected abstract IEnumerator WindDownGame();





}
