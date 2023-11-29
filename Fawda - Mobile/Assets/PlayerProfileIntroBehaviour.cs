using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerProfileIntroBehaviour : MonoBehaviour
{
    [SerializeField] TMP_Text nameInput; 
    // Start is called before the first frame update
    public void Start()
    {
     print("INTRO");
     PlayerProfileManager.singleton.createEvent.AddListener(() => ModalManager.singleton.SummonModal(1,"PlayerProfile_Intro"));
    }

    public void ProgressIntro(){
        print(nameInput.text);
        if(nameInput.text.Trim().Length == 0) return;
        
        PlayerProfileManager.singleton.StartNewProfile(new ProfileData(nameInput.text, 0));
        PlayerProfileManager.singleton.SaveProfile();
        PlayerProfileManager.singleton.LoadProfile();
        ModalManager.singleton.PlayAnimation("PlayerProfile_Outro");
        }
}
