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

    void EndTutorialLoop()
    {
        LobbyManager.singleton.StopCoroutine(tutorialEvent);
        tutorialEvent = null;
    }


    public virtual void ClearPlayers()
    {
        foreach (PlayerBehaviour player in playerInstances) player.Terminate();
        playerInstances = null;
    }

    public void BeginGame() => LobbyManager.singleton.StartCoroutine(EaseGameIn());


    public virtual void EndGame() => LobbyManager.singleton.StartCoroutine(WindDownGame());



    protected void LocateMapTransform() => transform = LobbyManager.gameManager.mapWrapper;

    protected abstract IEnumerator TutorialLoop();

    IEnumerator EaseGameIn()
    {
        EndTutorialLoop();
        ClearPlayers();
        SpawnPlayers();
        yield return new WaitForSeconds(0.15f);
        UIManager.singleton.SetCountdown("Ready?", _expiry: 0.8f, _color: new Color(0.6f, 0.05f, 0.02f), _showIndicator: true);
        UIManager.singleton.SetCountdown("Go!", _expiry: 0.5f, _color: new Color(0.05f, 0.6f, 0.02f), _callback: () =>
        {
            UIManager.bannerUIBehaviour.ClearBannerMessage();
            UIManager.bannerUIBehaviour.AddBannerMessage("Game in play", 1.2f);
            UIManager.bannerUIBehaviour.AddBannerMessage("BETA build", 1.2f);
        });
    }


    protected abstract IEnumerator WindDownGame();





}
