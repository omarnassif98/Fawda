using UnityEngine;

public class SafeAreaFitter
{
    RectTransform safeAreaRect, tabRect;

    static float TAB_AREA_TOP = 0.075f;

    public SafeAreaFitter(){
        DebugLogger.SourcedPrint("SafeAreaFitter","Awake");
        safeAreaRect = GameObject.Find("Canvas").transform.Find("Safe Area").GetComponent<RectTransform>();
        tabRect = GameObject.Find("Canvas").transform.Find("Tab Area").GetComponent<RectTransform>();
        CalibrateSafeArea();
    }

    public void CalibrateSafeArea(){
        Vector2 pos = Screen.safeArea.position;
        Vector2 extent = pos + Screen.safeArea.size;
        pos.x /= Screen.width;
        pos.y /= Screen.height;
        extent.x /= Screen.width;
        extent.y /= Screen.height;
        safeAreaRect.anchorMin = new Vector2(0,TAB_AREA_TOP);
        safeAreaRect.anchorMax = extent;
        if(tabRect == null){
            Debug.LogError("TabArea unset. Dipshit");
            return;
        }
        tabRect.gameObject.SetActive(true);
        tabRect.anchorMin = Vector2.zero;
        tabRect.anchorMax = new Vector2(1,TAB_AREA_TOP);
    }
}
