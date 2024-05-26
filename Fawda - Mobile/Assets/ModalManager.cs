using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalBehaviour
{
    Animator backdropAnimator;
    private UnityEvent dismissEvent = new UnityEvent();
    private Transform transform, modalWrapper;
    Button modalBackgroundDismiss;
    public ModalBehaviour(){
        transform = GameObject.Find("Canvas").transform.Find("Foreground");
        modalWrapper = transform.Find("Modal").Find("Safe Area");
        backdropAnimator = transform.GetComponent<Animator>();
        modalBackgroundDismiss = transform.Find("Backdrop").GetComponent<Button>();
        modalBackgroundDismiss.onClick.AddListener(DismissModal);
        modalBackgroundDismiss.GetComponent<Image>().raycastTarget = true;
    }

    public void SummonModal(string _modalScreenName){
        ClearModal();
        DebugLogger.SourcedPrint("Modal", "Bringing up " + _modalScreenName + " screen");
        GameObject skel = Resources.Load(String.Format("ModalScreens/{0}", _modalScreenName)) as GameObject;
        GameObject.Instantiate(skel, modalWrapper).name = _modalScreenName;
        modalBackgroundDismiss.gameObject.SetActive(true);
        backdropAnimator.SetBool("summoned", true);
    }

    void ClearModal(){
        for(int i = 0; i < modalWrapper.childCount; i++) GameObject.Destroy(modalWrapper.GetChild(i).gameObject);
    }

    public void AddDismissalListener(UnityAction _caller){
        dismissEvent.AddListener(_caller);
    }

    public void DismissModal(){
        DebugLogger.SourcedPrint("Modal", "Dismissing");
        modalBackgroundDismiss.gameObject.SetActive(true);
        backdropAnimator.SetBool("summoned", false);
        dismissEvent.Invoke();
        dismissEvent.RemoveAllListeners();
    }
}
