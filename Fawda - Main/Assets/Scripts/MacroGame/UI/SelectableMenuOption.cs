using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectableMenuOption : MonoBehaviour
{
    private short idx = -1;
    private ScreenManager controller;
    private bool frozen = false;

    [SerializeField]
    TMP_Text label;

    [SerializeField]
    Image background, image;

    [SerializeField]
    TMP_Text occupancyIndicator;


    [SerializeField]
    float RADIAL_DEFLATE_SECONDS = -1.2f, RADIAL_INFLATE_SECONDS = 3.6f;
    
    float radialFillAmount = 0, radialFillDelta = -1;
    
    [SerializeField]
    int occupiersNeeded = 1;

    int occupierCount = 0;

    
    public void Update(){
        if (frozen) return;
        Fill();
    }


    public void SetupButton(ScreenManager _controller, short _idx){
        controller = _controller;
        idx = _idx;
        controller.IntroductionEvent.AddListener(ResetButton);
        controller.DismissalEvent.AddListener(ScreenTransitionEventCallback);
    }

    //When screen is (re)introduced 
    public void ResetButton(){
        frozen = false;
        occupierCount = 0;
        radialFillAmount = 0;
        radialFillDelta = -1;
    }

    //First thing fired upon selection
    //Triggered when a button fills to 100%
    //It triggers an event which hits all siblings
    public void ScreenTransitionEventCallback(){
        GetComponent<Animator>().SetTrigger("Fire");
        frozen = true;
    }

    //Called by the variant of the fade out animation for the selected button
    //Nasically a callback from the last last last keyframe
    private void SelectionEventCallback(){
        if(idx == -1){
            Debug.LogError("EYYO DIPSHIT... UNCONFIGURED BUTTON");
            return;
        }

        controller.FireButtonCallback(idx);
        GetComponent<Animator>().SetBool("Selected", false);
    }


    //fired by cursors
    public void SetTarget(bool _occupancy){
        occupierCount += _occupancy?1:-1;
        occupierCount = occupierCount < 0?0:occupierCount;
        occupancyIndicator.text = string.Format("{0}/{1}",occupierCount.ToString(), occupiersNeeded.ToString());
        occupancyIndicator.color = occupierCount >= occupiersNeeded ? Color.white : Color.gray;
        radialFillDelta = occupierCount >= occupiersNeeded ? RADIAL_INFLATE_SECONDS:RADIAL_DEFLATE_SECONDS;
        GetComponent<Animator>().SetBool("Selected", occupierCount > 0);
    }

    public void Fill(){
        radialFillAmount +=(1/radialFillDelta) * Time.deltaTime;
        radialFillAmount = Mathf.Clamp(radialFillAmount,0,1);
        background.fillAmount = radialFillAmount;
        if(radialFillAmount < 1) return;
        MenuCursorManager.singleton.SetCursorInteractivities(false);
        controller.DismissalEvent.Invoke();
    }
}
