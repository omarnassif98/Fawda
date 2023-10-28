using UnityEngine;
using UnityEngine.UI;

public class ProfileColorSelectorButton : MonoBehaviour
{
    public static ProfileColorSelectorButton selectedChoice;
    [SerializeField]
    Sprite selectionImage;
    short color;
    [SerializeField]
    bool selected = false;

    //When populating the page, run 
    public void Setup(Color _color, short _colorIdx, bool _selected){
        GetComponent<Image>().color = _color;
        color = _colorIdx;

        if (_selected){
            selectedChoice = this;
            LockSelection();
        }
    }

    //Change sprite mask + singleton is now something else (another instance LockSelectioned)
    public void ReleaseSelection(){
        selected = false;
        GetComponent<Image>().sprite = null;
    } 


    //touched. Change singleton and sprite
    public void LockSelection(){
        if(selected) return;
        selectedChoice.ReleaseSelection();
        selectedChoice = this;
        selected = true;
        GetComponent<Image>().sprite = selectionImage;
    }
    //Idea is to just call this from singleton.
    //By def, singleton is the locked selected color
    public short GetColorIdx(){
        return color;
    }
}
