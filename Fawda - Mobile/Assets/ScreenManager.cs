using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ScreenManager
{
    private int currentSubscreenIdx =-1;
    public struct subscreenElement{
            public MaskableGraphic element;
            public float initialAlpha;
            public subscreenElement(MaskableGraphic _element){
                element = _element;
                initialAlpha = element.color.a;
            }
        }

    struct Subscreen{
        public GameObject gameObject;
        public List<subscreenElement> elements;
        public Subscreen(Transform _transform){
            gameObject = _transform.gameObject;
            TMP_Text[] textElements = _transform.GetComponentsInChildren<TMP_Text>();
            Image[] images = _transform.GetComponentsInChildren<Image>();
            elements = new List<subscreenElement>();
            foreach(TMP_Text txt in textElements) elements.Add(new subscreenElement(txt));
            foreach(Image img in images) elements.Add(new subscreenElement(img));
            DebugLogger.SourcedPrint("GameScreenManager", _transform.name + " has " + elements.Count.ToString() + " elements");

        }
    }

    private Subscreen[] subscreens;

    public ScreenManager(Transform _transform){
        currentSubscreenIdx = -1;
        subscreens = new Subscreen[_transform.childCount];
        for(int i = 0; i < subscreens.Length; i++) subscreens[i] = new Subscreen(_transform.GetChild(i));
        PlayerProfileManager.RegisterEphimeral(() => SwitchSubscreens(0),PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS);
    }

    public void SwitchSubscreens(int _newIdx){
        DebugLogger.SourcedPrint("ScreenManager","Switching");

        subscreens[_newIdx].gameObject.SetActive(true);
        foreach(subscreenElement elem in subscreens[_newIdx].elements) elem.element.color = new Color(elem.element.color.r,elem.element.color.g, elem.element.color.b,0);

        UnityAction<float> fadeinSetter = (progress) => {
            foreach(subscreenElement elem in subscreens[_newIdx].elements) elem.element.color = new Color(elem.element.color.r,elem.element.color.g, elem.element.color.b,progress * elem.initialAlpha);
        };

        UnityAction fadeinCallback = () => {
            if (currentSubscreenIdx != -1 ) subscreens[currentSubscreenIdx].gameObject.SetActive(false);
            currentSubscreenIdx = _newIdx;
        };

        UnityAction<float> fadeoutSetter = (progress) => {
            foreach(subscreenElement elem in subscreens[currentSubscreenIdx].elements) elem.element.color = new Color(elem.element.color.r,elem.element.color.g, elem.element.color.b,(1 - progress) * elem.initialAlpha);
        };

        UnityAction fadeoutCallback = () => {
            Orchestrator.singleton.StartCoroutine(Helper.LerpStep(fadeinSetter,fadeinCallback,0.1f));
        };
        if(currentSubscreenIdx == -1) Orchestrator.singleton.StartCoroutine(Helper.LerpStep(fadeinSetter,fadeinCallback,0.5f));
        else Orchestrator.singleton.StartCoroutine(Helper.LerpStep(fadeoutSetter,fadeoutCallback,0.1f));
    }
}
