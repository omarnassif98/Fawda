using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuScreenBehaviour : MonoBehaviour
{
    public bool isInteractable { get; private set; }
    LobbyMenuManager lobbyMenuManager;
    List<FloorButtonBehaviour> floorButtons;
    float lowestTimeRemaining;
    void Awake()
    {
        floorButtons = new List<FloorButtonBehaviour>();
        lobbyMenuManager = transform.parent.parent.GetComponent<LobbyMenuManager>();
        isInteractable = true;
        DebugLogger.SourcedPrint("LobbyMenuScreen","Awake");
    }

    public int FeedButton(FloorButtonBehaviour _floorButton){
        floorButtons.Add(_floorButton);
        return floorButtons.Count - 1;
    }


    void Update(){
        if(!isInteractable) return;
        foreach(FloorButtonBehaviour floorButton in floorButtons){
            float timeLeft = floorButton.Tick();
            if(timeLeft < FloorButtonBehaviour.maxTime) UIManager.singleton.SetCountdown(Mathf.CeilToInt(timeLeft));
            if(floorButton.IsFinished()) isInteractable = false;
        }
    }

    public void TriggerScreenChange(){
        ResetAllButtons();
    }

    void ResetAllButtons(){
        DebugLogger.SourcedPrint("LobbyMenuScreen","Reset all");
        foreach(FloorButtonBehaviour floorButtonActivations in floorButtons) floorButtonActivations.ResetTime();
    }

}
