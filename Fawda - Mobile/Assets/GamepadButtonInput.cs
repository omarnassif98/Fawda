using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GamepadButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pressed;
    [SerializeField]
    private Color idleColor, pressedColor;
    public void OnPointerDown(PointerEventData _eventData)
    {
        // Handle click event
        pressed = true;
        gameObject.GetComponent<Image>().color = pressedColor;
    }

    public void OnPointerUp(PointerEventData _eventData){
        pressed = false;
        gameObject.GetComponent<Image>().color = idleColor;
    }

    public bool PollInput(){
        return pressed;
    }
}
