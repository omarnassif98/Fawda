using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchStickInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{
    [SerializeField]
    private RectTransform cursorTransform, orientationTransform;
    private Vector2 stickVal;
    private bool tracking = false;
    float radius = 40;
    private float[] stickData = new float[2];
    private Vector3 pointerLocation;
    public void OnPointerDown(PointerEventData _eventData)
    {
        // Handle click event
        tracking = true;
        OnDrag(_eventData);
    }

    public void OnPointerUp(PointerEventData _eventData){
        tracking = false;
        cursorTransform.localPosition = Vector2.zero;
        stickVal = Vector2.zero;
        stickData[0] = stickData[1] = 0;
    }

    public void OnDrag(PointerEventData _eventData){
        pointerLocation = _eventData.position;
    }

    void Start(){
        radius = Mathf.Abs(Vector2.Distance(transform.position, orientationTransform.position));      
        print(radius);
    }
    void Update(){      
        if(!tracking) return;
        Vector3 offset = pointerLocation - transform.position;
        offset = Vector2.ClampMagnitude(offset,radius);
        //print(offset);
        float angle = Mathf.Deg2Rad * Vector2.SignedAngle(transform.right, offset);
        stickData[0] = angle;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector2 rotatedVector = rotation * offset;
        cursorTransform.position = transform.position + offset;
        stickData[1] = Mathf.Abs(Vector2.Distance(cursorTransform.position,transform.position)/radius);
        stickVal = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        //print(string.Format("Angle: {0}, val: {1}",angle, stickVal));
    }

    public float[] PollInput(){
        return stickData;
    }
}
