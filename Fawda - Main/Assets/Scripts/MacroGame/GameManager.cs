using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static GameManager singleton;
    private readonly Dictionary<GameCodes, Type> minigameLookup = new Dictionary<GameCodes,Type>()
    {
        {GameCodes.HAUNT, typeof(HauntGameDeployable)}
    };

    public static DeployableMinigame activeMinigame {get; private set;}

    [HideInInspector]
    public UnityEvent GameStartEvent = new UnityEvent(), GameEndEvent = new UnityEvent(), GamePauseEvent = new UnityEvent();


    void Awake(){
        Application.targetFrameRate = 30;
        if(singleton != null) Destroy(this);
        singleton = this;
    }

    void Start(){
        foreach (KeyValuePair<GameCodes, Type> entry in minigameLookup){
            DebugLogger.singleton.Log(string.Format("Game Manager Lookup Entry {0}: {1}", entry.Key, entry.Value));
        }
    }

    public void LoadMinigame(GameCodes _gamecode){
        if(activeMinigame != null) return;
        DebugLogger.SourcedPrint(gameObject.name,"Minigame Loaded");
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(minigameLookup[_gamecode]);
    }

    public void KillMinigame(){
        activeMinigame = null;
        DebugLogger.SourcedPrint(gameObject.name, "Minigame Killed");
    }

    public void ConfigureGame(Dictionary<string,int> _additionalConfig = null){
        Transform mapWrapper = transform.Find("MapWrapper");
        foreach(Transform go in mapWrapper) Destroy(go.gameObject);
        DebugLogger.SourcedPrint(gameObject.name, "Reset map");
        activeMinigame.SetupGame(mapWrapper, _additionalConfig);
    }
}
