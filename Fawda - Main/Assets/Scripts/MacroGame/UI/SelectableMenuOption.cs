using System;
using UnityEngine;
public class SelectableMenuOption : MonoBehaviour
{
    private bool focused;
    private short idx = -1;
    private ScreenManager controller;

    public void SetupButton(ScreenManager _controller, short _idx){
        controller = _controller;
        idx = _idx;
    }
    public void ActivateMenuOption(){
        if(idx == -1){
            Debug.LogError("EYYO DIPSHIT... UNCONFIGURED BUTTON");
            return;
        }
        controller.FireButtonCallback(idx);
    }
}
