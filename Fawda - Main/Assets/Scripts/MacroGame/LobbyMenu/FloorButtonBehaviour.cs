using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloorButtonBehaviour : MonoBehaviour
{
    private LobbyMenuScreenBehaviour screen;
    private int buttonIdx;
    [SerializeField] private int playersNeeded;
    short activations = 0;
    const float BASE_TIME = 5;
    public static float maxTime;
    float timeLeft;

    public void Start(){
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
       screen.TriggerScreenChange();
    }



    public void ResetTime(){
        timeLeft = ((LobbyManager.singleton.GetLobbySize() + 1) * BASE_TIME) - (activations * BASE_TIME);
    }

    void Update(){
        if(activations >= playersNeeded) timeLeft -= Time.deltaTime;
        else ResetTime();

        if(IsFinished()) return;
    }

    public bool IsFinished(){
        if(timeLeft > 0 ) return false;
        Trigger();
        ResetTime();
        return true;
    }
}
