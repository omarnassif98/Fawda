using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class Orchestrator : MonoBehaviour
{
    public void Start (){
        PlayerProfileManager.singleton.LoadProfile();
        DebugLogger.singleton.Log(string.Format("FYI a bool is {0} bytes long", BitConverter.GetBytes(true).Length));
        print("ORCHESTRA");
    }

    public void ResetData(){
        PlayerProfileManager.singleton.DeleteProfile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
