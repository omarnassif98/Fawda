using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuManager : MonoBehaviour
{
    GameObject lobbyMenuPlayerPrefab;
    LobbyMenuPlayerBehaviour[] lobbyMenuPlayerInstances;
    Animator snowglobeAnimator;
    Transform buttonParent;
    public static LobbyMenuManager singleton;
    ParticleSystem snowfallParticles, snowgustParticles;
    public static UnityEvent gustEvent;

    // Start is called before the first frame update

    void InferLobbySetup(){
        DebugLogger.SourcedPrint("LobbyMenu", "Inferring player data", ColorUtility.ToHtmlStringRGB(Color.cyan));
        ProfileData[] players = LobbyManager.players;
        for(int i = 0; i < players.Length; i++){
            if(players[i] != null && lobbyMenuPlayerInstances[i] == null) SpawnPlayer(i,players[i]);
        }
    }

    void Awake(){
        if (singleton != null) Destroy(gameObject);
        singleton = this;
        snowglobeAnimator = transform.GetComponent<Animator>();
        snowfallParticles = transform.Find("Snowfall").GetComponent<ParticleSystem>();
        snowgustParticles = transform.Find("Snowgust").GetComponent<ParticleSystem>();
        lobbyMenuPlayerPrefab = Resources.Load("Global/Prefabs/LobbyMenuPlayer") as GameObject;
        buttonParent = transform.Find("LobbyUIManager");
        gustEvent = new UnityEvent();
        DebugLogger.SourcedPrint("LobbyMenuManager","Awake");

    }

    void Start(){
        lobbyMenuPlayerInstances = new LobbyMenuPlayerBehaviour[LobbyManager.players.Length];
        InferLobbySetup();
        LobbyManager.singleton.playerJoinEvent.AddListener((idx) => SpawnPlayer(idx, LobbyManager.players[idx]));
        ConnectionManager.singleton.RegisterEphemeralServerEvent("wakeup", () => snowglobeAnimator.SetBool("capped",false));
        ConnectionManager.singleton.RegisterEphemeralServerEvent("wakeup", () => snowfallParticles.Stop());
    }

    public void ShakeSnowGlobe(){
        foreach(LobbyMenuPlayerBehaviour playerBehaviour in lobbyMenuPlayerInstances) if(playerBehaviour != null) playerBehaviour.PoofPlayer(false);
        snowglobeAnimator.SetTrigger("Shake");
    }

    [ContextMenu("Gust")]
    public void TriggerSnowGust(){
        gustEvent.Invoke();
        snowgustParticles.Clear();
        snowgustParticles.Play();
    }

    void SpawnPlayer(int _idx, ProfileData _loadout){
        DebugLogger.SourcedPrint("LobbyMenu", "Adding player", ColorUtility.ToHtmlStringRGB(Color.cyan));
        Vector3 spawnPoint = new Vector3(0, 0, 0);
        LobbyMenuPlayerBehaviour newPlayer = GameObject.Instantiate(lobbyMenuPlayerPrefab, spawnPoint, Quaternion.identity, transform).GetComponent<LobbyMenuPlayerBehaviour>();
        newPlayer.Initialize(_idx);
        lobbyMenuPlayerInstances[_idx] = newPlayer;
    }

    public void ShakeEnd(){
        PoofPlayers(true);
    }

    public void PoofPlayers(bool _val){
        foreach(LobbyMenuPlayerBehaviour lobbyMenuPlayerBehaviour in lobbyMenuPlayerInstances) if(lobbyMenuPlayerBehaviour != null) lobbyMenuPlayerBehaviour.PoofPlayer(_val);
        DioramaControllerBehaviour.singleton.SetCameraMode(_val);
    }

}
