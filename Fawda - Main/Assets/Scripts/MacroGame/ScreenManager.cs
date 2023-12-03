using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuNavigationOption{
    public SelectableMenuOption button;
    public ScreenManager destination;
}

public class ScreenManager : MonoBehaviour
{
    [SerializeField] MenuNavigationOption[] menuNavigationOptions;
    public UnityEvent IntroductionEvent, DismissalEvent;

    void Start(){
        for(short i = 0; i < menuNavigationOptions.Length; i++){
            menuNavigationOptions[i].button.SetupButton(this, i);
        }
        IntroductionEvent.AddListener(()=> MenuCursorManager.singleton.ToggleCursor(true));
        DismissalEvent.AddListener(()=> MenuCursorManager.singleton.ToggleCursor(false));
        DismissalEvent.AddListener(()=> UIManager.singleton.SwitchScreen());
        IntroductionEvent.Invoke();
    }

    public void FireButtonCallback(short _idx){
        if (!menuNavigationOptions[_idx].destination) return;
        menuNavigationOptions[_idx].destination.IntroductionEvent.Invoke();
        menuNavigationOptions[_idx].destination.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
