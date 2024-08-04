using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class LobbyMenuManager : MonoBehaviour
{
    public bool isInteractive { get; private set; }
    GameObject lobbyMenuPlayerPrefab;
    ParticleSystem bigPoof;
    LobbyMenuPlayerBehaviour[] lobbyMenuPlayerInstances;
    Animator snowglobeAnimator;
    Transform buttonParent;
    public static LobbyMenuManager singleton;
    ParticleSystem snowfallParticles, snowgustParticles;
    public static UnityEvent gustEvent;
    Transform currentFloorPlan;
    public Dictionary<string, UnityEvent> screenloadEvents { get; private set; }
    // Start is called before the first frame update



    void Awake(){
        if (singleton != null) Destroy(gameObject);
        isInteractive = false;
        singleton = this;
        snowglobeAnimator = transform.GetComponent<Animator>();
        snowfallParticles = transform.Find("Snowfall").GetComponent<ParticleSystem>();
        snowgustParticles = transform.Find("Snowgust").GetComponent<ParticleSystem>();
        lobbyMenuPlayerPrefab = Resources.Load("Global/Prefabs/LobbyMenuPlayer") as GameObject;
        buttonParent = transform.Find("LobbyUIManager");
        gustEvent = new UnityEvent();
        bigPoof = transform.Find("Big Poof").GetComponent<ParticleSystem>();
        DebugLogger.SourcedPrint("LobbyMenuManager","Awake");
        string resourcePath = Path.Combine(Application.dataPath, "Resources/LobbyMenuScreens");
        screenloadEvents = new Dictionary<string, UnityEvent>();
        foreach (string fname in Directory.GetFiles(resourcePath, "*.prefab")) screenloadEvents[new FileInfo(fname).Name.Split(".")[0]] = new UnityEvent();
        

    }


    void Start(){
        lobbyMenuPlayerInstances = new LobbyMenuPlayerBehaviour[LobbyManager.players.Length];
        InferLobbySetup();
        LobbyManager.singleton.playerJoinEvent.AddListener((idx) => SpawnPlayer(idx, LobbyManager.players[idx]));
        ConnectionManager.singleton.RegisterEphemeralServerEvent("wakeup", () => snowglobeAnimator.SetBool("capped",false));
        ConnectionManager.singleton.RegisterEphemeralServerEvent("wakeup", () => snowfallParticles.Stop());
        ConnectionManager.singleton.RegisterEphemeralServerEvent("wakeup", () => LoadScreen("GameSelectionScreen"));

    }

    public void ClearScreen()
    {
        if (currentFloorPlan == null) return;
        Destroy(currentFloorPlan.gameObject);
    }


    public void LoadScreen(string _screenName)
    {
        isInteractive = true;

        try
        {
            GameObject floorPlan = Resources.Load("LobbyMenuScreens/" + _screenName) as GameObject;
            currentFloorPlan = GameObject.Instantiate(floorPlan, transform).transform;
            currentFloorPlan.name = "LobbyMenuScreen";
            currentFloorPlan.transform.localPosition = Vector3.zero + Vector3.up  * 0.01f;
            screenloadEvents[_screenName].Invoke();
        }
        catch (System.Exception)
        {
            DebugLogger.SourcedPrint("LobbyMenuManager", "Could not load screen " + _screenName, "FF0000");
        }
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

    public void PoofPlayers(bool _val, bool _kill = false)
    {
        IEnumerator killer(GameObject gameObject)
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
        for (int i = 0; i < lobbyMenuPlayerInstances.Length; i++) { 
            LobbyMenuPlayerBehaviour lobbyMenuPlayerBehaviour = lobbyMenuPlayerInstances[i];
            if (lobbyMenuPlayerBehaviour == null) continue; //Player doesn't exist
            
            lobbyMenuPlayerBehaviour.PoofPlayer(_val);
            if (_val) DioramaControllerBehaviour.singleton.TrackTransform(lobbyMenuPlayerBehaviour.transform); //Poof in
            else DioramaControllerBehaviour.singleton.StopTrackTransform(lobbyMenuPlayerBehaviour.transform); //Poof out

            if (!_kill) continue; //We're not killing the menu players yet
            IEnumerator killEvent = killer(lobbyMenuPlayerBehaviour.gameObject);
            StartCoroutine(killEvent);
            lobbyMenuPlayerInstances[i] = null;
        }
    }

    public void PoofLobby()
    {
        bigPoof.Clear();
        bigPoof.Play();
    }

    void InferLobbySetup()
    {
        DebugLogger.SourcedPrint("LobbyMenu", "Inferring player data", ColorUtility.ToHtmlStringRGB(Color.cyan));
        ProfileData[] players = LobbyManager.players;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null && lobbyMenuPlayerInstances[i] == null) SpawnPlayer(i, players[i]);
        }
    }
}
