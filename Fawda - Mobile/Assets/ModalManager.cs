using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ModalManager : MonoBehaviour
{
    Animation backdropAnimator;
    [SerializeField] GameObject[] modalSubscreens;
    public static ModalManager singleton;
    private int activeScreen;
    private UnityEvent dismissEvent = new UnityEvent();
    
    [SerializeField] Button modalBackgroundDismiss;

    void Awake(){
        backdropAnimator = GetComponent<Animation>();
        if(singleton != null) Destroy(this);
        singleton = this;
    }
    // Start is called before the first frame update
    public void SummonModal(int _screenIdx, string _animation = "Modal_Intro"){
        print("Bringing up modal with " + modalSubscreens[_screenIdx].name + " screen");
        modalSubscreens[activeScreen].SetActive(false);
        modalSubscreens[_screenIdx].SetActive(true);
        PlayAnimation(_animation);
        activeScreen = _screenIdx;
        modalBackgroundDismiss.enabled = true;
    }

    public void AddDismissalListener(UnityAction _caller){
        dismissEvent.AddListener(_caller);
    }

    public void DismissModal(){
        modalBackgroundDismiss.enabled = false;
        modalSubscreens[activeScreen].SetActive(false);
        backdropAnimator.Play("Modal_Outro");
        dismissEvent.Invoke();
        dismissEvent.RemoveAllListeners();
    }

    public void PlayAnimation(string _animName){
        backdropAnimator.Play(_animName);
    }
}
