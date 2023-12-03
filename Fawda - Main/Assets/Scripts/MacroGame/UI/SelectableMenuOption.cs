using System;
using UnityEngine;
public class SelectableMenuOption : MonoBehaviour
{
    private short idx = -1;
    private ScreenManager controller;

    public void SetupButton(ScreenManager _controller, short _idx){
        controller = _controller;
        idx = _idx;
        controller.DismissalEvent.AddListener(ScreenTransitionEventCallback);
    }
    public void ActivateMenuOption(){
        GetComponent<Animator>().SetBool("Selected",true);
        controller.DismissalEvent.Invoke();
    }

    void ScreenTransitionEventCallback(){
        GetComponent<Animator>().SetTrigger("Fire");
    }


    public void SelectionEventCallback(){
        if(idx == -1){
            Debug.LogError("EYYO DIPSHIT... UNCONFIGURED BUTTON");
            return;
        }

        controller.FireButtonCallback(idx);
    }
}
