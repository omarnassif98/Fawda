using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent introEvent, outroEvent;

    public void IntroduceScreen(){
        gameObject.SetActive(true);
        print("INTRO PLZ");
        introEvent.Invoke();
    }

    public void DismissScreen(){
        gameObject.SetActive(false);
        outroEvent.Invoke();
    }

}
