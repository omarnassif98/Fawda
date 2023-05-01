using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchStickInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
    [SerializeField]
    private RectTransform cursorTransform;
    private Vector2 stickVal;
    private bool tracking = false;
    private int pointer_id;
    static float radius = 40;

    public void OnPointerDown(PointerEventData _eventData)
    {
        // Handle click event
        print(_eventData.pointerId);
        tracking = true;
        pointer_id = _eventData.pointerId;
    }

    public void OnPointerUp(PointerEventData _eventData){
        tracking = false;
        cursorTransform.localPosition = Vector2.zero;
    }

    void Update(){
        if(tracking){
            Vector3 offset = pointer_id == -1 ? Input.mousePosition - transform.position: Vector3.zero;
            offset = Vector2.ClampMagnitude(offset,radius);
            float angle = Vector2.Angle(transform.right, offset);
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            Vector2 rotatedVector = rotation * offset;
            cursorTransform.position = transform.position + offset;
            stickVal = offset * 1/radius;
        }
    }

    public Vector2 PollInput(){
        return stickVal;
    }
}
