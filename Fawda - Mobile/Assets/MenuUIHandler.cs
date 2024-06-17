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
        modalWrapper = modalTransform.Find("Modal").Find("Safe Area");
        modalBackgroundDismiss = modalTransform.Find("Backdrop").GetComponent<Button>();
        modalBackgroundDismiss.onClick.AddListener(DismissModal);
        modalBackgroundDismiss.GetComponent<Image>().raycastTarget = true;
    }

    public void SummonModal(string _modalScreenName){
        ClearModal();
        DebugLogger.SourcedPrint("Modal", "Bringing up " + _modalScreenName + " screen");
        GameObject skel = Resources.Load(String.Format("ModalScreens/{0}", _modalScreenName)) as GameObject;
        GameObject.Instantiate(skel, modalWrapper).name = _modalScreenName;
        modalTransform.gameObject.SetActive(true);
        animator.SetBool("Modal Summoned", true);
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
        IEnumerator nextStep = StepBackgroundColor(_newColor);
        Orchestrator.singleton.StartCoroutine(nextStep);
    }

    IEnumerator StepBackgroundColor(Color _colorToMatch){
        Color stepColor = Color.Lerp(Camera.main.backgroundColor, _colorToMatch, 1/60f);
        yield return new WaitForSeconds(1/60f);
        if (MathF.Abs((stepColor.r + stepColor.g + stepColor.b)/3 - (_colorToMatch.r + _colorToMatch.g + _colorToMatch.b)/3) <= 0.015f){
            Camera.main.backgroundColor = _colorToMatch;
            yield break;
        }
        Camera.main.backgroundColor = stepColor;
        IEnumerator nextStep = StepBackgroundColor(_colorToMatch);
        Orchestrator.singleton.StartCoroutine(nextStep);
    }
}
