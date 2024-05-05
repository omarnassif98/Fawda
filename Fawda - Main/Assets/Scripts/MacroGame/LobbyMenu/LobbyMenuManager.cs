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
    public Transform buttonParent;
    public static LobbyMenuManager singleton;
    ParticleSystem snowfallParticles, snowgustParticles;

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
        DebugLogger.SourcedPrint("LobbyMenuManager","Awake");

    }

    void Start(){
        lobbyMenuPlayerInstances = new LobbyMenuPlayerBehaviour[LobbyManager.players.Length];
        InferLobbySetup();
        LobbyManager.singleton.playerJoinEvent.AddListener((idx) => SpawnPlayer(idx, LobbyManager.players[idx]));
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => snowglobeAnimator.SetBool("capped",false));
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => snowfallParticles.Stop());
    }

    public void ShakeSnowGlobe(){
        foreach(LobbyMenuPlayerBehaviour playerBehaviour in lobbyMenuPlayerInstances) if(playerBehaviour != null) playerBehaviour.PoofPlayer();
        snowglobeAnimator.SetTrigger("Shake");
    }

    [ContextMenu("Gust")]
    public void TriggerSnowGust(){
        snowgustParticles.Clear();
        snowgustParticles.Play();
    }

    void SpawnPlayer(int _idx, ProfileData _loadout){
        DebugLogger.SourcedPrint("LobbyMenu", "Adding player", ColorUtility.ToHtmlStringRGB(Color.cyan));
        Vector3 spawnPoint = new Vector3(0, 0, 0);
        LobbyMenuPlayerBehaviour newPlayer = GameObject.Instantiate(lobbyMenuPlayerPrefab, spawnPoint, Quaternion.identity, transform).GetComponent<LobbyMenuPlayerBehaviour>();
        lobbyMenuPlayerInstances[_idx] = newPlayer;
    }


}
