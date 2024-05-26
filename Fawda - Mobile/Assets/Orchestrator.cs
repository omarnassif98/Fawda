using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
public class Orchestrator : MonoBehaviour
{
    public static Orchestrator singleton {get; private set;}
    public static InputHandler inputHandler { get; private set;}
    private ModalBehaviour modalBehaviour;
    private PlayerProfileManager playerProfileManager;
    private TabManager tabManager;
    void Awake(){
        if(singleton != null){ Destroy(this); return;}
        singleton = this;
        tabManager = new TabManager();
        inputHandler = new InputHandler();
        modalBehaviour = new ModalBehaviour();
        playerProfileManager = new PlayerProfileManager();

        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_FAILURE].AddListener(() => {
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].RemoveAllListeners();
            DebugLogger.SourcedPrint("Orchestrator", "Blank profile", "FF00000");
            modalBehaviour.SummonModal("Profile Creation");
            PlayerProfileIntroBehaviour profileCreator = new PlayerProfileIntroBehaviour();
            profileCreator.createEvent.AddListener((_name) => {
              playerProfileManager.StartNewProfile(new ProfileData(_name, 0));
              profileCreator = null;
              modalBehaviour.DismissModal();
            });
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.CREATED].AddListener(() => {
                tabManager.SwitchScreens(1);
                playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.CREATED].RemoveAllListeners();
            });
        });
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].AddListener(() => {
                tabManager.SwitchScreens(1);
                playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].RemoveAllListeners();
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
