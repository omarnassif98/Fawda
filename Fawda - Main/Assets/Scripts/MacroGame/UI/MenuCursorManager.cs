using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuCursorManager : MonoBehaviour
{
    public static MenuCursorManager singleton;
    TMP_Text cursorOperatorText;
    Image mainGraphicImage, radialProgressImage;
    float radialFillAmount = 0, radialFillDelta = 0;
    static float RADIAL_DEFLATE_SECONDS = -1.2f, RADIAL_INFLATE_SECONDS = 3.6f;
    bool occupied = false;
    float lastInteractionTime = 0;
    int occupierIdx;
    SelectableMenuOption target;

    
    void Awake(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        mainGraphicImage = transform.Find("Graphic").GetComponent<Image>();
        radialProgressImage = transform.Find("Graphic/Radial").GetComponent<Image>();
        radialProgressImage.fillAmount = 0;
        //mainGraphicImage.gameObject.SetActive(false);
        //radialProgressImage.gameObject.SetActive(false);
        ResourceManager.GetColors();
    }

    public void LockCursor(int playerIdx){
        occupied = true;
        occupierIdx = playerIdx;
        lastInteractionTime = Time.time;
        ToggleGraphic(); 
    }

    public void ReleaseCursor(){
        occupied = false;
        ToggleGraphic();
        radialProgressImage.fillAmount = 0;
        mainGraphicImage.rectTransform.position = Vector3.zero;
    }

    void ToggleGraphic(){
        mainGraphicImage.gameObject.SetActive(occupied);
        radialProgressImage.gameObject.SetActive(occupied);
    }

    public void MoveCursor(Vector2 _input){
        if(_input == Vector2.zero) return;
            /// FIX
            transform.Translate(_input * 100 * Time.deltaTime);
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
            print("I mean we got one");
            if(raycastResult[i].gameObject.GetComponent<SelectableMenuOption>()){
                radialFillDelta = 0;
                if (Time.time - lastInteractionTime < 0.2f) return;
                print("Filling");
                radialFillDelta = RADIAL_INFLATE_SECONDS;
                target = raycastResult[i].gameObject.GetComponent<SelectableMenuOption>();
                return;
            }
        }
        target = null;
        radialFillDelta = RADIAL_DEFLATE_SECONDS;
    }

    private void ProgressRadial(){
        radialFillAmount += 1/radialFillDelta * Time.deltaTime;
        radialFillAmount = Mathf.Clamp(radialFillAmount,0,1);
        radialProgressImage.fillAmount = radialFillAmount;
        print(radialFillAmount);
        if(radialFillAmount < 1) return;
        target.ActivateMenuOption();
        radialFillAmount = 0;
    }

    void Update(){

        //if(!occupied) return;
        ProbeUI();
        ProgressRadial();
    }
}
