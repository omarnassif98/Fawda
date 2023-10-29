using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabScreenRelation{
    public ScreenManager screenManager;
    public Button tabButton;
}
public class TabManager : MonoBehaviour
{
    [SerializeField] TabScreenRelation[] tabScreenRelations;
    int currentScreenIdx = 1;

    void Awake(){
        for(int i = 0; i < tabScreenRelations.Length; i++){
            int j = i;
            tabScreenRelations[j].tabButton.onClick.AddListener(() => SwitchScreens(j));
        }
        ActivateTab(currentScreenIdx);
    }

    public void SwitchScreens(int _newIdx){
        if(currentScreenIdx == _newIdx) return;
        DeactivateTab();
        ActivateTab(_newIdx);
        currentScreenIdx = _newIdx;
    }

    void ActivateTab(int _newIdx){
        tabScreenRelations[_newIdx].screenManager.gameObject.SetActive(true);
        tabScreenRelations[_newIdx].tabButton.GetComponent<Animation>().Play("Button_Puff");
    }

    void DeactivateTab(){
        tabScreenRelations[currentScreenIdx].screenManager.gameObject.SetActive(false);
        tabScreenRelations[currentScreenIdx].tabButton.GetComponent<Animation>().Play("Button_Shrink");
    }
}
