using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloorButtonBehaviour : MonoBehaviour
{
    private LobbyMenuScreenBehaviour screen;
    [SerializeField] UnityEvent triggerEvent;
    private int buttonIdx;
    short activations = 0;
    public void Start(){
        screen = transform.parent.GetComponent<LobbyMenuScreenBehaviour>();
        buttonIdx = screen.FeedButton(this);
        DebugLogger.SourcedPrint("LobbyMenuFloorButton","Awake");
    }

    void OnTriggerEnter(Collider _obj){
        if(_obj.tag != "playerCollider") return;
        activations ++;
        screen.ResetButton(buttonIdx);
    }

    void OnTriggerExit(Collider _obj){
        if(_obj.tag != "playerCollider") return;
        activations --;
        screen.ResetButton(buttonIdx);

    }

    void Update(){
        screen.SetButtonActivations(buttonIdx,activations);
    }
}
