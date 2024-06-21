using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.Events;
public class Orchestrator : MonoBehaviour
{
    public static Orchestrator singleton {get; private set;}
    public static InputHandler inputHandler { get; private set;}
    public PlayerProfileManager playerProfileManager {get; private set;}
    private MenuUIHandler menuUIHandler;
    private TabManager tabManager;
    public Dictionary<string, UnityAction> actionMap { get; private set;} = new Dictionary<string,UnityAction>();
    void Awake(){
        DebugLogger.SourcedPrint("Orchestrator", "Waking up", "770077");
        if(singleton != null){ Destroy(this); return;}
        singleton = this;
        playerProfileManager = new PlayerProfileManager();
        tabManager = new TabManager();
        inputHandler = new InputHandler();
        menuUIHandler = new MenuUIHandler();
        SafeAreaFitter safeAreaFitter = new SafeAreaFitter();
        InitializeProfile();
    }

    public void RegisterAction(string _key, UnityAction _action) => actionMap[_key] = _action;

    public void ExecuteAction(string _key) => actionMap[_key].Invoke();

    private void InitializeProfile() {
        UnityAction ephimeral = PlayerProfileManager.RegisterEphimeral(() => {
                menuUIHandler.SetTabVisibility(true);
                tabManager.SwitchScreens(1);
        }, PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS);


        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_FAILURE].AddListener(() => {
            playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].RemoveListener(ephimeral);
            InitiateProfileSetupFlow();
        });
        playerProfileManager.LoadProfile();
    }
    private void InitiateProfileSetupFlow(){
        menuUIHandler.FadeBackgroundColor(Color.black);
        DebugLogger.SourcedPrint("Orchestrator", "Blank profile", "FF00000");
        PlayerProfileIntroBehaviour profileCreator = new PlayerProfileIntroBehaviour();
        profileCreator.createEvent.AddListener((_name) => {
            playerProfileManager.StartNewProfile(new ProfileData(_name, 0));
            Destroy(profileCreator.flow.gameObject);
            profileCreator = null;
            menuUIHandler.SetTabVisibility(true);
        });

        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.CREATED].AddListener(() => {
            menuUIHandler.FadeBackgroundColor(new Color(0.31f,0.31f,0.31f));
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
