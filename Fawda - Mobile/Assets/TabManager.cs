using UnityEngine;
using UnityEngine.UI;

public class TabManager
{
    int currentScreenIdx = -1;
    struct TabScreenRelation{
        public Transform screen;
        public Animation tabButton;
        public TabScreenRelation(Transform _screen, Animation _tabButton) {
            screen = _screen;
            tabButton = _tabButton;
        }
    }
    private TabScreenRelation[] tabScreenRelations;



    public TabManager(){
        Transform tabTransform, screenTransform;
        tabTransform = GameObject.Find("Canvas").transform.Find("Tab Area/Tab Aligner");
        screenTransform = GameObject.Find("Canvas").transform.Find("Safe Area/Screens");

        if (tabTransform.childCount != screenTransform.childCount){
            DebugLogger.SourcedPrint("TabManager", "Not all screens accounted for", "FF00000");
            return;
        }

        tabScreenRelations = new TabScreenRelation[tabTransform.childCount];
        for(int i = 0; i < tabScreenRelations.Length; i++){
            int cpy = i;
            tabScreenRelations[cpy] = new TabScreenRelation(screenTransform.GetChild(cpy), tabTransform.GetChild(cpy).GetComponent<Animation>());
            DebugLogger.SourcedPrint("TabManager", "Relation " + cpy + "| Tab: " + tabScreenRelations[cpy].tabButton.name + "| Screen: " + tabScreenRelations[cpy].screen.name);
            tabTransform.GetChild(cpy).GetComponent<Button>().onClick.AddListener(() => SwitchScreens(cpy));
        }
    }

    public void SwitchScreens(int _newIdx){
            DebugLogger.SourcedPrint("TabManager", "Switch " + _newIdx, "FF00000");

        if(currentScreenIdx == _newIdx) return;
        if(currentScreenIdx >= 0) DeactivateTab();
        ActivateTab(_newIdx);
        currentScreenIdx = _newIdx;
    }

    void ActivateTab(int _newIdx){
        tabScreenRelations[_newIdx].screen.gameObject.SetActive(true);
        tabScreenRelations[_newIdx].tabButton.GetComponent<Animation>().Play("Button_Puff");
    }

    void DeactivateTab(){
        tabScreenRelations[currentScreenIdx].screen.gameObject.SetActive(false);
        tabScreenRelations[currentScreenIdx].tabButton.GetComponent<Animation>().Play("Button_Shrink");
    }
}
