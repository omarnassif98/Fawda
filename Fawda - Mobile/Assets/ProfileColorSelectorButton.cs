using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProfileColorSelectorButton
{
    [SerializeField]
    Sprite selectionImage;
    short color;
    [SerializeField]
    bool selected = false;
    UnityAction<int> Callback;

    [SerializeField] Image innerSelect, outerSelect;
    //When populating the page, run
    public void Setup(Color _color, short _colorIdx, bool _selected, UnityAction<int> _callback, UnityEvent<int> _event){
        innerSelect.color = _color;
        outerSelect.color = _color;
        color = _colorIdx;
        Callback = _callback;
        _event.AddListener(LockInSelection);
    }

    //Change sprite mask + singleton is now something else (another instance LockSelectioned)

    public void LockInSelection(int _idx){
        outerSelect.color = (_idx == color)?Color.black:PlayerProfileManager.singleton.GetColors()[color];
    }


    //touched. Change singleton and sprite
    public void LockSelection(){
        Callback(color);
    }

}
