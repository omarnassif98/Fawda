using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [SerializeField]
    TMP_Text hertzDisplay;
    private float radius;

    public void Initialize(short _idx){
        //Radius calculated with invisible object
        radius = Mathf.Abs(Vector2.Distance(transform.GetChild(1).position,  orientation.position));
        playerIdx = _idx; 
    }



    void Update(){
        UpdateVisualization(InputManager.singleton.PullJoypadState(playerIdx));
        hertzDisplay.text = string.Format("{0} Hz", InputManager.singleton.GetIndexHertz(playerIdx).ToString("F2"));
    }

    public void UpdateVisualization(JoypadState _controls){
        Vector2 _joystickInput = _controls.analog * radius;
        bool _buttonInput = _controls.action;
        cursor.position = new Vector2(joystick.position.x + _joystickInput.x, joystick.position.y + _joystickInput.y);
        buttonIndicator.color = _buttonInput?activeColor:idleColor;
    }
}
