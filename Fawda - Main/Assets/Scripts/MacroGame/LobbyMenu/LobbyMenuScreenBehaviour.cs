using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuScreenBehaviour : MonoBehaviour
{
    List<FloorButtonBehaviour> floorButtons;
    UnityAction screenloadAction;
    Dictionary<string, UnityAction> customButtonActions;
    float lowestTimeRemaining;
    public Dictionary<ActionType, UnityAction<string>> buttonActions;

    void Awake()
    {
        floorButtons = new List<FloorButtonBehaviour>();
        DebugLogger.SourcedPrint("LobbyMenuScreen","Awake");
        buttonActions = new Dictionary<ActionType, UnityAction<string>>();
        customButtonActions = new Dictionary<string, UnityAction>();
        buttonActions[ActionType.SCREEN_LOAD] = LobbyMenuManager.singleton.LoadScreen;
        buttonActions[ActionType.GAME_SETUP] = (string _code) => {
            LobbyManager.gameManager.LoadMinigame((GameCodes)Enum.Parse(typeof(GameCodes), _code));
            LobbyMenuManager.singleton.ClearScreen();
            LobbyMenuManager.singleton.PoofPlayers(false);
            };
    }

    void Start(){
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => LobbyMenuManager.singleton.LoadScreen("GameSelectionScreen"));
    }







    public int FeedButton(FloorButtonBehaviour _floorButton){
        floorButtons.Add(_floorButton);
        _floorButton.SetVisibility(true);
        return floorButtons.Count - 1;
    }


    void Update(){
        if(!LobbyMenuManager.singleton.isInteractive) return;
        foreach(FloorButtonBehaviour floorButton in floorButtons){
            float timeLeft = floorButton.Tick();
            if(timeLeft < FloorButtonBehaviour.maxTime) UIManager.singleton.SetCountdown(Mathf.CeilToInt(timeLeft));
            if(floorButton.IsFinished()) {
                return;
            }
        }
    }

    

}
