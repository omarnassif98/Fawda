using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuScreenBehaviour : MonoBehaviour
{
    public bool isInteractable { get; private set; }
    List<FloorButtonBehaviour> floorButtons;

    Dictionary<string, UnityAction> customButtonActions;
    float lowestTimeRemaining;
    Transform currentFloorPlan;
    public Dictionary<ActionType, UnityAction<string>> buttonActions;

    void Awake()
    {
        floorButtons = new List<FloorButtonBehaviour>();
        isInteractable = true;
        DebugLogger.SourcedPrint("LobbyMenuScreen","Awake");
        buttonActions = new Dictionary<ActionType, UnityAction<string>>();
        customButtonActions = new Dictionary<string, UnityAction>();
        buttonActions[ActionType.SCREEN_LOAD] = LoadScreen;
        customButtonActions["Shake"] = () => LobbyMenuManager.singleton.ShakeSnowGlobe();
        buttonActions[ActionType.CUSTOM_ACTION] = (string _str) => customButtonActions[_str]();
    }

    void Start(){
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => LoadScreen("MainScreen"));

    }
    public void LoadScreen(string _screenName){
        ClearScreen();
        try
        {
            GameObject floorPlan = Resources.Load("LobbyMenuScreens/" + _screenName) as GameObject;
            currentFloorPlan = GameObject.Instantiate(floorPlan,transform).transform;
            currentFloorPlan.transform.localPosition = Vector3.zero;
        }
        catch (System.Exception)
        {
            DebugLogger.SourcedPrint("LobbyMenuManager","Could not load screen " + _screenName, "FF0000");
        }
    }

    public void ClearScreen(){
        if(currentFloorPlan == null) return;
        foreach(FloorButtonBehaviour floorButton in floorButtons) floorButton.SetVisibility(false);
        Destroy(currentFloorPlan.gameObject,2);
        floorButtons.Clear();
        currentFloorPlan = null;
    }


    public int FeedButton(FloorButtonBehaviour _floorButton){
        floorButtons.Add(_floorButton);
        _floorButton.SetVisibility(true);
        return floorButtons.Count - 1;
    }


    void Update(){
        if(!isInteractable) return;
        foreach(FloorButtonBehaviour floorButton in floorButtons){
            float timeLeft = floorButton.Tick();
            if(timeLeft < FloorButtonBehaviour.maxTime) UIManager.singleton.SetCountdown(Mathf.CeilToInt(timeLeft));
            if(floorButton.IsFinished()) {
                isInteractable = false;
                return;
            }
        }
    }

}
