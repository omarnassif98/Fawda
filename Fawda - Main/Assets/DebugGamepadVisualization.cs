using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugGamepadVisualization : MonoBehaviour
{
    [SerializeField]
    RectTransform joystick, cursor;
    [SerializeField]
    Image buttonIndicator, pulseIndicator;
    [SerializeField]
    Color idleColor, activeColor;
    [SerializeField]
    short playerIdx;

    bool trackingState = false;

    private float radius = 40;

    public void SetTrackingState(bool _state){
        print("TRACKING");
        trackingState = _state; 
    }

    void Update(){
        if(trackingState) UpdateVisualization(InputManager.singleton.PullJoypadState(playerIdx));
    }

    public void UpdateVisualization(JoypadState _controls){
        FlickerIndicator();
        Vector2 _joystickInput = _controls.analog * radius;
        bool _buttonInput = _controls.action;
        cursor.position = new Vector2(joystick.position.x + _joystickInput.x, joystick.position.y + _joystickInput.y);
        buttonIndicator.color = _buttonInput?activeColor:idleColor;
    }

    void FlickerIndicator(){
        pulseIndicator.color = activeColor;
        StartCoroutine(switch_off_indicator());
    }

    IEnumerator switch_off_indicator(){
        yield return new WaitForSeconds(0.05f);
        pulseIndicator.color = idleColor;
    }
}
