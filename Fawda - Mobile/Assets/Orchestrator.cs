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
    private MenuUIHandler menuUIHandler;
    private PlayerProfileManager playerProfileManager;
    private TabManager tabManager;
    void Awake(){
        DebugLogger.SourcedPrint("Orchestrator", "Waking up", "770077");
        if(singleton != null){ Destroy(this); return;}
        singleton = this;
        tabManager = new TabManager();
        inputHandler = new InputHandler();
        menuUIHandler = new MenuUIHandler();
        playerProfileManager = new PlayerProfileManager();
        SafeAreaFitter safeAreaFitter = new SafeAreaFitter();

        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_FAILURE].AddListener(() => {
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].RemoveAllListeners();
            InitiateProfileSetupFlow();
        });
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].AddListener(() => {
                menuUIHandler.SetTabVisibility(true);
                tabManager.SwitchScreens(1);
                playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].RemoveAllListeners();
            });
        playerProfileManager.LoadProfile();
    }

    private void InitiateProfileSetupFlow(){
        DebugLogger.SourcedPrint("Orchestrator", "Blank profile", "FF00000");
        PlayerProfileIntroBehaviour profileCreator = new PlayerProfileIntroBehaviour();
        profileCreator.createEvent.AddListener((_name) => {
            playerProfileManager.StartNewProfile(new ProfileData(_name, 0));
            Destroy(profileCreator.flow.gameObject);
            profileCreator = null;
            menuUIHandler.SetTabVisibility(true);
        });

        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.CREATED].AddListener(() => {
            tabManager.SwitchScreens(1);
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.CREATED].RemoveAllListeners();
        });
    }

    public void ResetData(){
        playerProfileManager.DeleteProfile();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update(){
        inputHandler.PollInput();
    }
}
