using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        logs.Enqueue(string.Format("\n{0}",newLine));
    }

    public static void SourcedPrint(string _src, string _msg, string _color = "FFFFFF", [CallerFilePath] string _path = "", [CallerMemberName] string callingMethod = "", [CallerLineNumber] int callingFileLineNumber = 0){
        string log = String.Format("<b><color=#{2}>{0}: </color></b>{1}", _src, _msg, _color);
        string pathFromProjectFolder = _path.Split("Fawda - Mobile/")[1];

        Debug.Log(log + string.Format("\n<a href=\"{0}\" line=\"{1}\">Take me there</a>", _path, callingFileLineNumber));
        if(singleton!=null) singleton.Log(log);
    }

    public void Update(){
        if(logs.Count == 0) return;
        log.text += logs.Dequeue();
    }
}
