using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private Animator[] buttons;
    [SerializeField]
    private short[] nextScreens;
    public bool active = false;
    private static float BUTTON_OUTRO_LENGTH = 0.5f;
    private static float BUTTON_INTRO_LENGTH = 0.5f;
    [SerializeField]
    private int activeButtonIdx = 0;
    [SerializeField]
    private UnityEvent screenAction;
    void Start(){
        buttons[activeButtonIdx].SetBool("focused",true);
        ConnectionManager.singleton.RegisterRPC("MENU_CONTROL", ManipulateMenu);
    }
    
    void ManipulateMenu(byte[] _data){
        short dir = (short) _data[0];
        switch(dir){
            case 1:
            case 2:
                buttons[activeButtonIdx].SetBool("focused",false);
                activeButtonIdx += (dir == 1)?-1:1;
                if(activeButtonIdx < 0) activeButtonIdx += buttons.Length;
                activeButtonIdx = activeButtonIdx % buttons.Length;
                buttons[activeButtonIdx].SetBool("focused",true);
                break;
            case 3:
                UIManager.singleton.SwitchScreens(nextScreens[activeButtonIdx]);
                break;
            case 4:
                UIManager.singleton.PopScreen();
                break;
        }
    }

    public void IntroduceScreen(){
        IEnumerator first = RevealButton(0);
        StartCoroutine(first);
    }

    public void PerformScreenAction(){
        screenAction.Invoke();
    }

    public void DismissScreen(bool linger = true){
        for(short i = 0;  i < buttons.Length; i++){
            if(linger == true && buttons[i].GetBool("focused") == true){
                IEnumerator delay = HideLingeringButton(i);
                StartCoroutine(delay);
            }else{
                buttons[i].SetBool("showing", false);
            }
        }

        if(linger == false){
            IEnumerator delay = WaitAndInvoke(UIManager.singleton.screenClearEvent, BUTTON_OUTRO_LENGTH);
            StartCoroutine(delay);
        }
    }

    private IEnumerator RevealButton(short _idx){
        buttons[_idx].SetBool("showing", true);
        yield return new WaitForSeconds(0.05f);
        _idx++;
        if(_idx < buttons.Length){
            IEnumerator next = RevealButton(_idx);
            StartCoroutine(next);
        }else{
            IEnumerator delay = WaitAndInvoke(UIManager.singleton.screenFillEvent, BUTTON_INTRO_LENGTH);
            StartCoroutine(delay);
        }
    }

    private IEnumerator HideLingeringButton(short _idx){
        yield return new WaitForSeconds(0.10f);
        buttons[_idx].SetBool("showing", false);
        IEnumerator delay = WaitAndInvoke(UIManager.singleton.screenClearEvent, BUTTON_OUTRO_LENGTH);
        StartCoroutine(delay);
    }

    private IEnumerator WaitAndInvoke(UnityEvent _event, float _waitTime){
        yield return new WaitForSeconds(_waitTime);
        _event.Invoke();
    }
}
