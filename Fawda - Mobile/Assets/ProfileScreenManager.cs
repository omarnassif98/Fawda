using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileScreenManager : ScreenManager
{
    [SerializeField] TMP_InputField nameDisplay;
    [SerializeField] Image nameBackground;
    [SerializeField] PlayerColorPickerManager playerColorPickerManager;

    public void Start(){
        print("From beyond the fucking grave");
        PlayerProfileManager.singleton.loadEvent.AddListener(fillName);
        PlayerProfileManager.singleton.loadEvent.AddListener(FillCharacterColor);

        fillName();
        FillCharacterColor();
    }

    void fillName(){
        print(PlayerProfileManager.singleton.GetProfileData().name);
        nameDisplay.text = PlayerProfileManager.singleton.GetProfileData().name;
    }

    public void ChangeName(){
        ProfileData prof = PlayerProfileManager.singleton.GetProfileData();
        if(nameDisplay.text == prof.name) return;
        prof.name = nameDisplay.text;
        PlayerProfileManager.singleton.SetProfileData(prof);
    }

    public void FillCharacterColor(){
        int idx = PlayerProfileManager.singleton.GetProfileData().colorSelection;
        Color newColor = PlayerProfileManager.singleton.GetColors()[idx];
        nameBackground.color = newColor;
        playerColorPickerManager.ChangeColorSelection(idx);
    }

    public void ChangeCharacterColor(int _colorSelectionIdx){
        ProfileData prof = PlayerProfileManager.singleton.GetProfileData();
        if(_colorSelectionIdx == prof.colorSelection) return;
        prof.colorSelection = _colorSelectionIdx;
        PlayerProfileManager.singleton.SetProfileData(prof);
    }
}
