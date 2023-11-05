using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Data.Odbc;
public class MenuCursorManager : MonoBehaviour
{
    public static MenuCursorManager singleton;
    TMP_Text cursorOperatorText;
    Image mainGraphicImage, radialProgressImage;
    float radialFillAmount = 0, radialFillDelta = 0;
    static float RADIAL_DEFLATE_SECONDS = -1.2f, RADIAL_INFLATE_SECONDS = 3.6f;
    bool occupied = false;
    float lastInteractionTime = 0;
    int occupierIdx = -1;
    SelectableMenuOption target;
    Vector2 joypadState;
    
    
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
        ConnectionManager.singleton.RegisterRPC( OpCode.MENU_CONTROL, UpdateJoypad);
        ConnectionManager.singleton.RegisterRPC( OpCode.MENU_OCCUPY,HandleClientOccupation);
        ConnectionManager.singleton.RegisterRPC(OpCode.MENU_OCCUPATION_STATUS,GetCursorOccupancy);
        mainGraphicImage = transform.Find("Graphic").GetComponent<Image>();
        radialProgressImage = transform.Find("Graphic/Radial").GetComponent<Image>();
        radialProgressImage.fillAmount = 0;
        ToggleGraphic();
        ResourceManager.GetColors();
    }

    public void GetCursorOccupancy(byte[] _data = null, int _idx = -1){
        byte[] occBytes = BitConverter.GetBytes(occupied);
        byte[] idxBytes = BitConverter.GetBytes(occupierIdx);
        byte[] resBytes = new byte[occBytes.Length + idxBytes.Length];
        print(string.Format("Cursor Occ: {0} bytes ", resBytes.Length));
        Buffer.BlockCopy(occBytes, 0, resBytes,0,occBytes.Length);
        Buffer.BlockCopy(idxBytes, 0, resBytes, occBytes.Length,idxBytes.Length);
        NetMessage ans = new NetMessage(OpCode.MENU_OCCUPATION_STATUS, resBytes);
        ConnectionManager.singleton.SendMessageToClients(ans, _idx);
        }

    public void HandleClientOccupation(byte[] _data, int _idx){
        bool occ = BitConverter.ToBoolean(_data,0);
        if (occ && occupied) {
            GetCursorOccupancy();
            return;
        }
        
        occupied = occ;
        occupierIdx = _idx;
        print(string.Format("occ = {0} idx = {1}",occ,occupierIdx));
        lastInteractionTime = Time.time;
        joypadState = Vector2.zero;
        radialProgressImage.fillAmount = 0;
        GetComponent<RectTransform>().position = new Vector2(Screen.width/2,Screen.height/2);
        ToggleGraphic(); 
        GetCursorOccupancy();
    }


    void ToggleGraphic(){
        mainGraphicImage.gameObject.SetActive(occupied);
        radialProgressImage.gameObject.SetActive(occupied);
    }


    public void UpdateJoypad(byte[] _data, int _idx){
        print(string.Format("occ = {0} idx = {1}",occupierIdx,_idx));
        if(occupierIdx != _idx) return;
        float dirInput = BitConverter.ToSingle(_data,0);
        float distInput = BitConverter.ToSingle(_data,4);
        joypadState = new Vector2(Mathf.Cos(dirInput), Mathf.Sin(dirInput)) * distInput;
    }

    private void MoveCursor(){
        transform.Translate(joypadState * 100 * Time.deltaTime);
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
                radialFillDelta = 0;
                if (Time.time - lastInteractionTime < 0.2f) return;
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
        if(radialFillAmount < 1) return;
        target.ActivateMenuOption();
        radialFillAmount = 0;
    }

    void Update(){

        if(!occupied) return;
        ProbeUI();
        ProgressRadial();
        MoveCursor();
    }
}
