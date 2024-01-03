using System;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
public class SelectableMenuOption : MonoBehaviour
{
    private short idx = -1;
    private ScreenManager controller;

    public bool frozen = false;

    [SerializeField]
    TMP_Text label;

    [SerializeField]
    Image background, image;

    [SerializeField]
    TMP_Text occupancyIndicator;


    static float RADIAL_DEFLATE_SECONDS = -1.2f, RADIAL_INFLATE_SECONDS = 3.6f;
    float radialFillAmount = 0, radialFillDelta = RADIAL_DEFLATE_SECONDS;
    int occupierCount = 0;

    public void Update(){
        Fill();
    }


    public void SetupButton(ScreenManager _controller, short _idx){
        controller = _controller;
        idx = _idx;
        controller.DismissalEvent.AddListener(ScreenTransitionEventCallback);
    }
    public void ActivateMenuOption(){
        frozen = true;
        controller.DismissalEvent.Invoke();
    }

    void ScreenTransitionEventCallback(){
        frozen = GetComponent<Animator>().GetBool("Selected");
        GetComponent<Animator>().SetTrigger("Fire");
        radialFillAmount = 0;
    }


    public void SelectionEventCallback(){
        if(idx == -1){
            Debug.LogError("EYYO DIPSHIT... UNCONFIGURED BUTTON");
            return;
        }

        controller.FireButtonCallback(idx);
        occupierCount = 0;
        GetComponent<Animator>().SetBool("Selected", false);
        frozen = false;
    }


    public void SetTarget(bool _occupancy){
        if(frozen) return;
        occupierCount += _occupancy?1:-1;
        occupierCount = occupierCount < 0?0:occupierCount;
        occupancyIndicator.text = occupierCount.ToString();
        GetComponent<Animator>().SetBool("Selected", occupierCount > 0);
        radialFillDelta = occupierCount > 0? RADIAL_INFLATE_SECONDS:RADIAL_DEFLATE_SECONDS;
    }

    public void Fill(){
        radialFillAmount += (frozen?0:(1/radialFillDelta)) * Time.deltaTime;
        radialFillAmount = Mathf.Clamp(radialFillAmount,0,1);
        background.fillAmount = radialFillAmount;
        if(radialFillAmount >= 1) ActivateMenuOption();
    }
}
