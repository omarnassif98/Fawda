using UnityEngine;
public class PlayerColorPickerManager : MonoBehaviour
{
    [SerializeField]
    GameObject colorPickerPrefab;
    public void Start(){
        Color[] colors = PlayerProfileManager.singleton.GetColors();
        for(short i=0; i < colors.Length; i++){
            GameObject col = GameObject.Instantiate(colorPickerPrefab, Vector3.zero,Quaternion.identity,transform);
            col.GetComponent<ProfileColorSelectorButton>().Setup(colors[i], i, i==0);
        }
    }    
}
