using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerProfileManager
{
    public enum PROFILE_MANAGER_ACTIONS{
        LOAD_SUCCESS=1,
        LOAD_FAILURE=2,
        CREATED=3,
        SAVED=4
    }
    public Dictionary<PROFILE_MANAGER_ACTIONS, UnityEvent> profileManagerEvents {get; private set;}

    public ProfileData playerProfile {get; private set;}
    SaveFileHandler profileHandler;

    public PlayerProfileManager()
    {
        profileManagerEvents = new Dictionary<PROFILE_MANAGER_ACTIONS, UnityEvent>();;
        profileHandler = new SaveFileHandler(Application.persistentDataPath, "prof.json");
        foreach(PROFILE_MANAGER_ACTIONS profAction in Enum.GetValues(typeof(PROFILE_MANAGER_ACTIONS))) profileManagerEvents[profAction] = new UnityEvent();
    }

    public static UnityAction RegisterEphimeral(UnityAction _action, PROFILE_MANAGER_ACTIONS _wait){
        UnityAction ephimeral = null;
        ephimeral = () => {
            DebugLogger.SourcedPrint("PlayerProfileManager","Ephimeral Spent");
            _action();
            Orchestrator.singleton.playerProfileManager.profileManagerEvents[_wait].RemoveListener(ephimeral);
        };
        Orchestrator.singleton.playerProfileManager.profileManagerEvents[_wait].AddListener(ephimeral);
        return ephimeral;
    }

    public void LoadProfile(){
        playerProfile = profileHandler.Load();
        if(playerProfile != null) profileManagerEvents[PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].Invoke();
        else profileManagerEvents[PROFILE_MANAGER_ACTIONS.LOAD_FAILURE].Invoke();
        DebugLogger.singleton.Log("Profile Loaded");
    }

    public void SaveProfile(){
        profileHandler.Save(playerProfile);
        profileManagerEvents[PROFILE_MANAGER_ACTIONS.SAVED].Invoke();
    }

    public void StartNewProfile(ProfileData _profile){
        playerProfile = _profile;
        profileManagerEvents[PROFILE_MANAGER_ACTIONS.CREATED].Invoke();
        profileManagerEvents[PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].Invoke();
        SaveProfile();
    }

    public void DeleteProfile(){
        profileHandler.Delete();
        playerProfile = null;

    }

    public ProfileData GetProfileData(){
        return playerProfile;
    }

    public void SetProfileData(ProfileData _newProfile){
        playerProfile = _newProfile;
        SaveProfile();
    }

}
