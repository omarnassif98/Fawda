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

    public void UpdateVisualization(Vector2 _joystickInput, bool _buttonInput){
        cursor.position = new Vector2(joystick.position.x + _joystickInput.x, joystick.position.y + _joystickInput.y);
        buttonIndicator.color = _buttonInput?activeColor:idleColor;
    }

    IEnumerator switch_off_indicator(){
        yield return new WaitForSeconds(0.05f);
        pulseIndicator.color = idleColor;
    }
}
