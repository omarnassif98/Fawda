using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuNavigationOption{
    public SelectableMenuOption button;
    public ScreenManager destination;
    public UnityEvent action;
}

public class ScreenManager : MonoBehaviour
{
    [SerializeField] MenuNavigationOption[] menuNavigationOptions;


    void Start(){
        for(short i = 0; i < menuNavigationOptions.Length; i++){
            menuNavigationOptions[i].button.SetupButton(this, i);
        }
    }

    public void FireButtonCallback(short _idx){
        menuNavigationOptions[_idx].action.Invoke();
        if (!menuNavigationOptions[_idx].destination) return;
        UIManager.singleton.SwitchScreen();
        menuNavigationOptions[_idx].destination.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
