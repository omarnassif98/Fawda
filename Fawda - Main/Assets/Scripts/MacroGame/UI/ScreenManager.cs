using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuNavigationOption{
    public SelectableMenuOption button;
    public UnityEvent clickEvent;
}

public class ScreenManager : MonoBehaviour
{
    [SerializeField] MenuNavigationOption[] menuNavigationOptions;
    public UnityEvent IntroductionEvent, DismissalEvent;

    void Awake(){
        for(short i = 0; i < menuNavigationOptions.Length; i++){
            menuNavigationOptions[i].button.SetupButton(this, i);
        }
    }

    void OnEnable(){
        IntroductionEvent.Invoke();
    }
    
    public void FireButtonCallback(short _idx){
        print("CALLBACK " + _idx);
        menuNavigationOptions[_idx].clickEvent.Invoke();
    }

}
