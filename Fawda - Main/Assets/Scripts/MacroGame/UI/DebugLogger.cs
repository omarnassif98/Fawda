using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    public static DebugLogger singleton;
    [SerializeField] TMP_Text log;
    public void Awake(){
        if (singleton != null) return;
        singleton = this;
    }
    // Start is called before the first frame update
    public void Log(string newLine){
        log.text += string.Format("\n{0}",newLine);
    }
}
