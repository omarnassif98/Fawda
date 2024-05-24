using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class Orchestrator : MonoBehaviour
{
    public static Orchestrator singleton;
    public InputHandler inputHandler { get; private set;}
    public static ModalBehaviour modalBehaviour;
    public void Start (){
        PlayerProfileManager.singleton.LoadProfile();
        DebugLogger.singleton.Log(string.Format("FYI a bool is {0} bytes long", BitConverter.GetBytes(true).Length));
        print("ORCHESTRA");
    }
    void Awake(){
        if(singleton != null){ Destroy(this); return;}
        singleton = this;
        inputHandler = new InputHandler();
        modalBehaviour = new ModalBehaviour(GameObject.Find("Canvas").transform.Find("Foreground"));
    }

    public void ResetData(){
        PlayerProfileManager.singleton.DeleteProfile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update(){
        inputHandler.PollInput();
    }
}
