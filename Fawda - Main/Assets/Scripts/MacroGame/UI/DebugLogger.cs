using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    public static DebugLogger singleton;
    [SerializeField] TMP_Text log;
    Queue<string> logs = new Queue<string>();
    public void Awake(){
        if (singleton != null) return;
        singleton = this;
    }
    // Start is called before the first frame update
    public void Log(string newLine){
        print(newLine);
        logs.Enqueue(string.Format("\n{0}",newLine));
    }

    public void Update(){
        if(logs.Count == 0) return;
        log.text += logs.Dequeue();
    }
}
