using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] RectTransform tabRect;
    
    static float TAB_AREA_TOP = 0.075f;


    // Start is called before the first frame update
    void Awake(){
        rectTransform = GetComponent<RectTransform>();
        Vector2 pos = Screen.safeArea.position;
        Vector2 extent = pos + Screen.safeArea.size;
        pos.x /= Screen.width;
        pos.y /= Screen.height;
        extent.x /= Screen.width;
        extent.y /= Screen.height;
        rectTransform.anchorMin = new Vector2(0,TAB_AREA_TOP);
        rectTransform.anchorMax = extent;
        if(tabRect == null){
            Debug.LogError("TabArea unset. Dipshit");
            return;
        }
        tabRect.gameObject.SetActive(true);
        tabRect.anchorMin = Vector2.zero;
        tabRect.anchorMax = new Vector2(1,TAB_AREA_TOP);
    }
}
