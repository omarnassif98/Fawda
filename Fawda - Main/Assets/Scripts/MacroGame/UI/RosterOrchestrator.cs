using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ScreenOrchestrator
{
    ScreenManager currentScreen = null;
    Transform screenParent;

    public ScreenOrchestrator(Transform _parent){
        screenParent = _parent;
    }

    public void ChangeScreen(string _screenName){
        if(currentScreen != null){
            currentScreen.gameObject.SetActive(false);
        }
        ScreenManager newScreen = screenParent.Find(_screenName).GetComponent<ScreenManager>();
        newScreen.gameObject.SetActive(true);
        currentScreen = newScreen;
    }

}
