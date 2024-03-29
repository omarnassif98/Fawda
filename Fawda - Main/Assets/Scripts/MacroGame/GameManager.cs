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
        {GameCodes.HAUNT, typeof(HauntGameManager)}
    };

    [SerializeField]
    private DeployableMinigame activeMinigame;

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

        UIManager.singleton.backgroundBehaviour.idealCheckerboardOpacity = 0;
    }

    public void LoadMinigame(GameCodes _gamecode){
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(minigameLookup[_gamecode]);
    }

    public void KillMinigame(){
        activeMinigame = null;
    }

    public void ConfigureGame(int _specialIdx = -1){
        Transform mapWrapper = transform.Find("MapWrapper");
        foreach(Transform go in mapWrapper) Destroy(go.gameObject);
        activeMinigame.SetupGame(mapWrapper, _specialIdx);
        UIManager.singleton.backgroundBehaviour.JoltBackground();
        UIManager.singleton.backgroundBehaviour.idealCheckerboardOpacity = 0;
    }
}
