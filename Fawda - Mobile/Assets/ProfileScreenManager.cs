using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileScreenManager : ScreenManager
{
    TMP_InputField nameDisplay;
    Image nameBackground;
    private PlayerProfileManager playerProfileManager;

    public ProfileScreenManager(Transform _transform) : base(_transform)
    {
        DebugLogger.SourcedPrint("ProfileScreen","Awake");
        Transform wrapper = _transform.GetChild(0);
        nameBackground = wrapper.Find("Nametag/Background").GetComponent<Image>();
        nameDisplay = wrapper.Find("Nametag").GetComponent<TMP_InputField>();
        nameDisplay.onEndEdit.AddListener(ChangeName);
        playerProfileManager = Orchestrator.singleton.playerProfileManager;
        playerProfileManager.profileManagerEvents[PlayerProfileManager.PROFILE_MANAGER_ACTIONS.LOAD_SUCCESS].AddListener(FillProfileDetails);
    }

    public void FillProfileDetails(){
        DebugLogger.SourcedPrint("ProfileScreen","Fill Event","00FFFF");
        ProfileData profileData = playerProfileManager.GetProfileData();
        SetName(profileData.name);
    }

    public void SetName(string _name){
        DebugLogger.SourcedPrint("ProfileScreen","Loaded name is " + _name,"00FFFF");
        nameDisplay.text = _name;
    }

    public void ChangeName(string _newName){
        DebugLogger.SourcedPrint("ProfileScreen","New name is " + _newName,"00FFFF");
        ProfileData prof = playerProfileManager.GetProfileData();
        if(_newName == prof.name) return;
        prof.name = _newName;
        playerProfileManager.SetProfileData(prof);
    }

    public void FillCharacterColor(){
        int idx = playerProfileManager.GetProfileData().colorSelection;
        //Color newColor = playerProfileManager.GetColors()[idx];
        //nameBackground.color = newColor;
        //playerColorPickerManager.ChangeColorSelection(idx);
    }

    public void ChangeCharacterColor(int _colorSelectionIdx){
        ProfileData prof = playerProfileManager.GetProfileData();
        if(_colorSelectionIdx == prof.colorSelection) return;
        prof.colorSelection = _colorSelectionIdx;
        playerProfileManager.SetProfileData(prof);
    }
}
