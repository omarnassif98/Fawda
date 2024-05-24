using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalBehaviour
{
    Animation backdropAnimator;
    private UnityEvent dismissEvent = new UnityEvent();
    private Transform transform, modalWrapper;
    Button modalBackgroundDismiss;

    public ModalBehaviour(Transform _transform){
        transform = _transform;
        modalWrapper = transform.Find("Modal");
        backdropAnimator = transform.GetComponent<Animation>();
        modalBackgroundDismiss = transform.Find("Background").GetComponent<Button>();

    }

    public void SummonModal(string _modalScreenName){
        ClearModal();
        DebugLogger.SourcedPrint("Modal", "Bringing up " + _modalScreenName + " screen");
        GameObject skel = Resources.Load(String.Format("ModalScreens/{0}", _modalScreenName)) as GameObject;
        GameObject.Instantiate(skel, modalWrapper);
        modalBackgroundDismiss.enabled = true;
    }

    void ClearModal(){
        for(int i = 0; i < modalWrapper.childCount; i++) GameObject.Destroy(modalWrapper.GetChild(i));
    }

    public void AddDismissalListener(UnityAction _caller){
        dismissEvent.AddListener(_caller);
    }

    public void DismissModal(){
        modalBackgroundDismiss.enabled = false;
        backdropAnimator.Play("Modal_Outro");
        dismissEvent.Invoke();
        dismissEvent.RemoveAllListeners();
    }

    public void PlayAnimation(string _animName){
        backdropAnimator.Play(_animName);
    }
}
