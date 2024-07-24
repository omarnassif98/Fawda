using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum ActionType{
        SCREEN_LOAD = 0,
        GAME_SETUP = 1,
        CUSTOM_ACTION = 2
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
    SpriteRenderer foregroundSprite, graphicSprite;
    ParticleSystem poofParticles;
    [SerializeField] private float idealAlpha = 0, realAlpha = 0;


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

    void Awake(){
        foregroundSprite = GetComponent<SpriteRenderer>();
        graphicSprite = transform.Find("Graphic").GetComponent<SpriteRenderer>();
        poofParticles = transform.Find("Smoke Particles").GetComponent<ParticleSystem>();
    }

    public void Start(){
        maxTime = ((LobbyManager.singleton.GetLobbySize()) * BASE_TIME) - (activations * BASE_TIME);
        timeLeft = BASE_TIME;
        screen = transform.parent.parent.GetComponent<LobbyMenuScreenBehaviour>();
        buttonIdx = screen.FeedButton(this);
    }

    public void SetVisibility(bool _isVisible){
        idealAlpha = _isVisible? 1:0;
    }

    void OnTriggerEnter(Collider _obj){
        if(!_obj.GetComponent<PlayerBehaviour>()) return;
        activations ++;
        OnOccupancy();
        ResetTime();
    }

    void OnTriggerExit(Collider _obj){
        if(!_obj.GetComponent<PlayerBehaviour>()) return;
        activations --;
        OnOccupancy();
        ResetTime();

    }

    public void OnOccupancy(){
        foregroundSprite.color = (activations == 0)?new Color(0,0,0,realAlpha):new Color(0.75f,0.75f,0.75f,realAlpha);
        graphicSprite.color = (activations > 0)?new Color(0,0,0,realAlpha):new Color(0.75f,0.75f,0.75f,realAlpha);
    }

    public void Trigger(){
        foreach(FloorAction floorAction in callback) screen.buttonActions[floorAction.action](floorAction.value);
    }



    public void ResetTime(){
        timeLeft = maxTime;
    }

    public float Tick(){
        if(activations >= playersNeeded) timeLeft -= Time.deltaTime;
        else ResetTime();
        return timeLeft;
    }
    public void StepAlpha(){
        realAlpha = (Mathf.Abs(idealAlpha - realAlpha) < 0.005f)?idealAlpha:Mathf.Lerp(realAlpha,idealAlpha,0.2f);
        foregroundSprite.color = new Color(foregroundSprite.color.r, foregroundSprite.color.g, foregroundSprite.color.b, realAlpha);
        graphicSprite.color = new Color(graphicSprite.color.r, graphicSprite.color.g, graphicSprite.color.b, realAlpha);
    }
    public bool IsFinished(){
        if(timeLeft > 0 ) return false;
        Trigger();
        ResetTime();
        return true;
    }
    void Update(){
        StepAlpha();
    }
}
