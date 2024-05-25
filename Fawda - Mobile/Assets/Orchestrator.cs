using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
public class Orchestrator : MonoBehaviour
{
    public static Orchestrator singleton;
    public InputHandler inputHandler { get; private set;}
    private ModalBehaviour modalBehaviour;
    private PlayerProfileManager playerProfileManager;

    void Awake(){
        if(singleton != null){ Destroy(this); return;}
        singleton = this;
        inputHandler = new InputHandler();
        modalBehaviour = new ModalBehaviour();
        playerProfileManager = new PlayerProfileManager();

        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_FAILURE].AddListener(() => {
            DebugLogger.SourcedPrint("Orchestrator", "Blank profile", "FF00000");
            PlayerProfileIntroBehaviour profileCreator = new PlayerProfileIntroBehaviour();
            modalBehaviour.SummonModal("Profile Creation");
            profileCreator.createEvent.AddListener((_name) => {
              playerProfileManager.StartNewProfile(new ProfileData(_name, 0));
              profileCreator = null;
            });
        });
        playerProfileManager.LoadProfile();
        DebugLogger.SourcedPrint("Orchestrator", "awake", "770077");
    }



    public void ResetData(){
        playerProfileManager.DeleteProfile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update(){
        inputHandler.PollInput();
    }
}
