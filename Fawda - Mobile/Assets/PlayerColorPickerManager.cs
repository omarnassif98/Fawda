using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//WHY DOES THIS EVEN EXIST??
public class PlayerColorPickerManager : MonoBehaviour
{
    [SerializeField]
    GameObject colorPickerPrefab;

    [SerializeField]
    ProfileScreenManager profileScreenManager;

    UnityEvent<int> buttonPressEvent = new UnityEvent<int>();
    public void Start(){
        /*Color[] colors = PlayerProfileManager.singleton.GetColors();
        for(short i=0; i < colors.Length; i++){
            GameObject col = GameObject.Instantiate(colorPickerPrefab, Vector3.zero,Quaternion.identity,transform);
            col.GetComponent<ProfileColorSelectorButton>().Setup(colors[i], i, i==0, ChangeColorSelection, buttonPressEvent);
        }
        */
    }

    public void ChangeColorSelection(int _selection){
        buttonPressEvent.Invoke(_selection);
        profileScreenManager.ChangeCharacterColor(_selection);
    }

}
