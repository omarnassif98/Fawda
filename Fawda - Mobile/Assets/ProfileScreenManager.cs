using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileScreenManager : ScreenManager
{
    [SerializeField] TMP_Text nameDisplay;
    [SerializeField] Image nameBackground;
    [SerializeField] PlayerColorPickerManager playerColorPickerManager; // WHAT THE FUCK??
    private PlayerProfileManager playerProfileManager;

    public ProfileScreenManager(Transform _transform) : base(_transform)
    {
        Transform wrapper = _transform.GetChild(0);
        nameBackground = wrapper.Find("Nametag/Background").GetComponent<Image>();
        nameDisplay = wrapper.Find("Nametag/Name").GetComponent<TMP_Text>();
    }

    public void SetProfileManagerInstance(PlayerProfileManager _playerProfileManager){
        playerProfileManager = _playerProfileManager;
    }
    public void SetName(string _name){
        nameDisplay.text = _name;
    }

    public void ChangeName(){
        ProfileData prof = playerProfileManager.GetProfileData();
        if(nameDisplay.text == prof.name) return;
        prof.name = nameDisplay.text;
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
