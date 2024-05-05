using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum ActionType{
        SCREEN_LOAD = 0,
        CUSTOM_ACTION = 1
    }

public class FloorButtonBehaviour : MonoBehaviour
{
    private LobbyMenuScreenBehaviour screen;
    private int buttonIdx;
    [SerializeField] private int playersNeeded;
    short activations = 0;
    const float BASE_TIME = 5;
    public static float maxTime;
    float timeLeft;


    [Serializable] struct FloorAction
    {
        public ActionType action;
        public string value;
        public FloorAction(ActionType _action, string _value){
            action = _action;
            value = _value;
        }
    }

    [SerializeField] private FloorAction[] callback;


    public void Start(){
        maxTime = ((LobbyManager.singleton.GetLobbySize()) * BASE_TIME) - (activations * BASE_TIME);
        timeLeft = BASE_TIME;
        screen = transform.parent.GetComponent<LobbyMenuScreenBehaviour>();
        buttonIdx = screen.FeedButton(this);
    }

    void OnTriggerEnter(Collider _obj){
        if(_obj.tag != "playerCollider") return;
        activations ++;
        OnOccupancy();
        ResetTime();
    }

    void OnTriggerExit(Collider _obj){
        if(_obj.tag != "playerCollider") return;
        activations --;
        OnOccupancy();
        ResetTime();

    }

    public void OnOccupancy(){
        GetComponent<SpriteRenderer>().color = (activations == 0)?Color.black:Color.white*0.15f;
    }

    public void Trigger(){
        foreach(FloorAction floorAction in callback) LobbyMenuManager.buttonActions[floorAction.action](floorAction.value);
    }



    public void ResetTime(){
        timeLeft = maxTime;
    }

    public float Tick(){
        if(activations >= playersNeeded) timeLeft -= Time.deltaTime;
        else ResetTime();
        return timeLeft;
    }

    public bool IsFinished(){
        if(timeLeft > 0 ) return false;
        Trigger();
        ResetTime();
        return true;
    }
}
