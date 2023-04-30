using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamepadButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed;

    public void OnPointerDown(PointerEventData _eventData)
    {
        // Handle click event
        print("Pointer DOWN");
        pressed = true;
    }

    public void OnPointerUp(PointerEventData _eventData){
        print("Pointer UP");
        pressed = false;
    }

    public bool PollInput(){
        return pressed;
    }
}
