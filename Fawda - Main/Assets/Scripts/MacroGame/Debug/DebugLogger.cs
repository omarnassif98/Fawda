using System;
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

    [HideInCallstack]
    public static void SourcedPrint(string _src, string _msg, string _color = "FFFFFF"){
        string log = String.Format("<b><color=#{2}>{0}: </color></b>{1}", _src, _msg, _color);
        print(log);
        if(singleton!=null) singleton.Log(log);
    }
    public void Log(string newLine){
        logs.Enqueue(string.Format("\n{0}",newLine));
    }

    public void Update(){
        if(logs.Count == 0) return;
        log.text += logs.Dequeue();
    }
}
