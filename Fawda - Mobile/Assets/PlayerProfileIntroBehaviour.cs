using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerProfileIntroBehaviour
{
    [SerializeField] TMP_Text nameInput;
    // Start is called before the first frame update
    public PlayerProfileIntroBehaviour(TMP_Text _nameInput)
    {
     nameInput = _nameInput;
    }

    public void ProgressIntro(){
        if(nameInput.text.Trim().Length == 0) return;
        PlayerProfileManager.singleton.StartNewProfile(new ProfileData(nameInput.text, 0));
        PlayerProfileManager.singleton.SaveProfile();
        PlayerProfileManager.singleton.LoadProfile();
        }
}
