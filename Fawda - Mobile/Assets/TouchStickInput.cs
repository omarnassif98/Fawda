using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TouchStickInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{
    [SerializeField]
    private RectTransform cursorTransform, orientationTransform;
    private bool tracking = false;
    float radius = 40;
    private Vector3 pointerLocation;
    [SerializeField] TMP_Text debugVal;
    public void OnPointerDown(PointerEventData _eventData)
    {
        // Handle click event
        tracking = true;
        OnDrag(_eventData);
    }

    public void OnPointerUp(PointerEventData _eventData){
        tracking = false;
        cursorTransform.localPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData _eventData){
        pointerLocation = _eventData.position;
    }

    void Start(){
        radius = Mathf.Abs(Vector2.Distance(transform.position, orientationTransform.position));
        print(radius);
    }
    void Update(){
        if(!tracking){
            FinalizeOutput(Vector2.zero);
            return;
        }
        Vector3 offset = pointerLocation - transform.position;
        offset = Vector2.ClampMagnitude(offset,radius);
        float angle = Mathf.Deg2Rad * Vector2.SignedAngle(orientationTransform.position - transform.position, offset);
        cursorTransform.position = transform.position + offset;
        float power = Mathf.Clamp01(Mathf.Abs(Vector2.Distance(cursorTransform.position,transform.position)/radius));
        Vector2 input = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * power;
        FinalizeOutput(input);
    }

    void FinalizeOutput(Vector2 _newVal){
        Orchestrator.singleton.inputHandler.stickVal = _newVal;
        debugVal.text = string.Format("X: {0:0.00}\nY: {1:0.00} ", Orchestrator.singleton.inputHandler.stickVal.x, Orchestrator.singleton.inputHandler.stickVal.y);
    }

}
