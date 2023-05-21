using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugGamepadVisualization : MonoBehaviour
{
    [SerializeField]
    RectTransform joystick, cursor, orientation;
    [SerializeField]
    Image buttonIndicator;
    [SerializeField]
    Color idleColor, activeColor;
    [SerializeField]
    short playerIdx;

    bool trackingState = false;
    private float radius;

    public void Start(){
        //Radius calculated with invisible object
        radius = Mathf.Abs(Vector2.Distance(transform.GetChild(1).position,  orientation.position)); 
    }

    public void SetTrackingState(bool _state){
        //basically is the stick touched
        trackingState = _state; 
    }

    void Update(){
        if(trackingState) UpdateVisualization(InputManager.singleton.PullJoypadState(playerIdx));
    }

    public void UpdateVisualization(JoypadState _controls){
        Vector2 _joystickInput = _controls.analog * radius;
        bool _buttonInput = _controls.action;
        cursor.position = new Vector2(joystick.position.x + _joystickInput.x, joystick.position.y + _joystickInput.y);
        buttonIndicator.color = _buttonInput?activeColor:idleColor;
    }
}
