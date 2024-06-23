using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuUIHandler
{
    Animator animator;
    private UnityEvent dismissEvent = new UnityEvent();
    private Transform modalTransform, modalWrapper;
    Button modalBackgroundDismiss;
    public MenuUIHandler(){
        animator = GameObject.Find("Canvas").GetComponent<Animator>();
        modalTransform = GameObject.Find("Canvas").transform.Find("Foreground");
        modalWrapper = modalTransform.Find("Modal/Safe Area");
        modalTransform.Find("Modal/DismissButton").GetComponent<Button>().onClick.AddListener(DismissModal);
        modalBackgroundDismiss = modalTransform.Find("Backdrop").GetComponent<Button>();
        modalBackgroundDismiss.onClick.AddListener(DismissModal);
        modalBackgroundDismiss.GetComponent<Image>().raycastTarget = true;
        Orchestrator.singleton.RegisterAction("Summon Tab", () => SetTabVisibility(true));
        Orchestrator.singleton.RegisterAction("Dismiss Tab", () => SetTabVisibility(false));
    }

    public Transform SummonModal(string _modalScreenName){
        ClearModal();
        DebugLogger.SourcedPrint("Modal", "Bringing up " + _modalScreenName + " screen");
        GameObject skel = Resources.Load(String.Format("DeployableScreens/{0}", _modalScreenName)) as GameObject;
        GameObject.Instantiate(skel, modalWrapper).name = _modalScreenName;
        modalTransform.gameObject.SetActive(true);
        animator.SetBool("Modal Summoned", true);
        return modalWrapper.transform;
    }

    void ClearModal(){
        for(int i = 0; i < modalWrapper.childCount; i++) GameObject.Destroy(modalWrapper.GetChild(i).gameObject);
    }

    public void AddDismissalListener(UnityAction _caller){
        dismissEvent.AddListener(_caller);
    }

    public void DismissModal(){
        DebugLogger.SourcedPrint("Modal", "Dismissing");
        modalTransform.gameObject.SetActive(false);
        animator.SetBool("Modal Summoned", false);
        dismissEvent.Invoke();
    }

    public void SetTabVisibility(bool _visible){
        animator.SetBool("Tab Summoned", _visible);
    }
    public void FadeBackgroundColor(Color _newColor){
    DebugLogger.SourcedPrint("MenuUI", "COLOR");
    Color startColor = Camera.main.backgroundColor;
    Orchestrator.singleton.StartCoroutine(Helper.LerpStep((_progress) => Camera.main.backgroundColor = Color.Lerp(startColor, _newColor, _progress),_time:0.5f));
    }
}
