using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuScreenBehaviour : MonoBehaviour
{
    public bool isInteractable { get; private set; }
    LobbyMenuManager lobbyMenuManager;

    class FloorButtonActivations{
        const float BASE_TIME = 5;
        public static float maxTime;
        float timeLeft;
        public FloorButtonBehaviour floorButton;
        public short activations;
        public FloorButtonActivations(FloorButtonBehaviour _floorButton, short _activations){
            floorButton = _floorButton;
            activations = _activations;
            timeLeft = BASE_TIME;
            ResetTime();
        }

        public void ResetTime(){
            timeLeft = ((LobbyManager.singleton.GetLobbySize() + 1) * BASE_TIME) - (activations * BASE_TIME);
            DebugLogger.SourcedPrint("FloorButtonActivation", string.Format("Time Needed {0}", timeLeft), "00ff00");
        }

        public void Tick(){
            ResetTime();
            return;
            if(activations > 0) timeLeft -= Time.deltaTime;
            else ResetTime();
        }
    }

    List<FloorButtonActivations> floorButtons;

    void Awake()
    {
        floorButtons = new List<FloorButtonActivations>();
        lobbyMenuManager = transform.parent.GetComponent<LobbyMenuManager>();
        isInteractable = true;
        DebugLogger.SourcedPrint("LobbyMenuScreen","Awake");
    }

    public int FeedButton(FloorButtonBehaviour _floorButton){
        floorButtons.Add(new FloorButtonActivations(_floorButton, 0));
        return floorButtons.Count - 1;
    }

    public void SetButtonActivations(int _idx, short _activations){
        floorButtons[_idx].activations = _activations;
    }

    void Update(){
        if(!isInteractable) return;
        foreach(FloorButtonActivations floorButton in floorButtons) floorButton.Tick();
    }

    public void ResetButton(int _idx){
        floorButtons[_idx].ResetTime();
    }
}
