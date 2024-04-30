using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuManager : MonoBehaviour
{
    GameObject lobbyMenuPlayerPrefab;
    LobbyMenuPlayerBehaviour[] lobbyMenuPlayerInstances;
    Animator icebergAnimator;


    private Transform buttonParent;

    // Start is called before the first frame update

    void InferLobbySetup(){
        DebugLogger.SourcedPrint("LobbyMenu", "Inferring player data", ColorUtility.ToHtmlStringRGB(Color.cyan));
        ProfileData[] players = LobbyManager.players;
        for(int i = 0; i < players.Length; i++){
            if(players[i] != null && lobbyMenuPlayerInstances[i] == null) SpawnPlayer(i,players[i]);
        }
    }

    void Awake(){
        icebergAnimator = transform.Find("Floor").GetComponent<Animator>();
        lobbyMenuPlayerPrefab = Resources.Load("Global/Prefabs/LobbyMenuPlayer") as GameObject;
        buttonParent = transform.Find("LobbyUIManager");
        DebugLogger.SourcedPrint("LobbyMenuManager","Awake");
    }

    void Start(){
        lobbyMenuPlayerInstances = new LobbyMenuPlayerBehaviour[LobbyManager.players.Length];
        InferLobbySetup();
        LobbyManager.singleton.playerJoinEvent.AddListener((idx) => SpawnPlayer(idx, LobbyManager.players[idx]));
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => LoadScreen("MainScreen"));

    }


    void SpawnPlayer(int _idx, ProfileData _loadout){
        DebugLogger.SourcedPrint("LobbyMenu", "Adding player", ColorUtility.ToHtmlStringRGB(Color.cyan));
        Vector3 spawnPoint = new Vector3(0, 0, 0);
        LobbyMenuPlayerBehaviour newPlayer = GameObject.Instantiate(lobbyMenuPlayerPrefab, spawnPoint, Quaternion.identity, transform).GetComponent<LobbyMenuPlayerBehaviour>();
        lobbyMenuPlayerInstances[_idx] = newPlayer;
    }

    public void LoadScreen(string _screenName){
        for(int i = 0; i < buttonParent.childCount; i++) Destroy(buttonParent.GetChild(i));
        try
        {
            GameObject floorPlan = Resources.Load("LobbyMenuScreens/" + _screenName) as GameObject;
            Transform floor = GameObject.Instantiate(floorPlan,buttonParent).transform;
            transform.localPosition = Vector3.zero;
        }
        catch (System.Exception)
        {
            DebugLogger.SourcedPrint("LobbyMenuManager","Could not load screen " + _screenName, "FF0000");
        }
    }

    public void TriggerFlip(){
        foreach(LobbyMenuPlayerBehaviour lobbyMenuPlayer in lobbyMenuPlayerInstances) if(lobbyMenuPlayer != null) lobbyMenuPlayer.EmitSmoke();
        icebergAnimator.SetTrigger("flip");
        ClearScreen();
    }

    public void ClearScreen(){
        for(int i = 0; i < buttonParent.childCount; i++) Destroy(buttonParent.GetChild(i).gameObject);
    }
}
