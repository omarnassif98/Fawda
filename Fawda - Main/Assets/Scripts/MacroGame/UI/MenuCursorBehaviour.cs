using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuCursorBehaviour : MonoBehaviour
{

    private TMP_Text cursorOperatorText;
    private Image mainGraphicImage;
    const float MOVE_SPEED = 500;
    bool occupied = false, interactionEnabled = false;
    float lastInteractionTime = 0;
    SelectableMenuOption target;
    Vector2 joypadState;


    // Start is called before the first frame update
    void Start()
    {
        cursorOperatorText = transform.Find("playername").GetComponent<TMP_Text>();
        mainGraphicImage = transform.Find("Graphic").GetComponent<Image>();
        ToggleGraphic();
    }

    public void UpdateGraphics(string _newName, Color _newColor){
        cursorOperatorText.text = _newName;
        mainGraphicImage.color = _newColor;
    }

    public void HandleOccupation(bool _newStatus){
        occupied = _newStatus;
        lastInteractionTime = Time.time;
        joypadState = Vector2.zero;
        GetComponent<RectTransform>().position = new Vector2(Screen.width/2,Screen.height/2);
        ToggleGraphic();
    }

    public void ToggleCursorInteractivity(bool _enabled){
        interactionEnabled = _enabled;
    }

    void ToggleGraphic(){
        mainGraphicImage.gameObject.SetActive(occupied);
        cursorOperatorText.gameObject.SetActive(occupied);
    }


    public void UpdateJoypad(float _dir, float _dist){
        joypadState = new Vector2(Mathf.Cos(_dir), Mathf.Sin(_dir)) * _dist;
    }

    private void MoveCursor(){
        GetComponent<Animator>().SetBool("Primed", !(target==null));
        if (joypadState == Vector2.zero) return;
        transform.Translate(joypadState * MOVE_SPEED * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x,GetComponent<RectTransform>().rect.width/2,Screen.width - GetComponent<RectTransform>().rect.width/2), Mathf.Clamp(transform.position.y,GetComponent<RectTransform>().rect.height/2,Screen.height - GetComponent<RectTransform>().rect.height/2), 0);
        lastInteractionTime = Time.time;
    }

    private void ProbeUI(){
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        eventData.position = (corners[0] + corners[2]) / 2f;

        List<RaycastResult> raycastResult = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResult);
        for (int i = 0; i < raycastResult.Count; i++)
        {
            if(raycastResult[i].gameObject.GetComponent<SelectableMenuOption>()){
                if (target == raycastResult[i].gameObject.GetComponent<SelectableMenuOption>() || !interactionEnabled) return;
                target = raycastResult[i].gameObject.GetComponent<SelectableMenuOption>();
                target.SetTarget(true);
                return;
            }
        }
        if(target != null) target.SetTarget(false);
        target = null;
    }


    void Update(){

        if(!occupied) return;
        ProbeUI();
        MoveCursor();
    }
}
