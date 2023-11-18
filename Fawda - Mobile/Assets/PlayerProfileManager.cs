using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


[System.Serializable]
public class ProfileData{
    public string name;
    public int colorSelection;
    public int topCustomization;
    public int midCustomization;
    public int botCustomization;

    public ProfileData(string _name, int _colorSelection){
        this.name = _name;
        this.colorSelection = _colorSelection;
        this.topCustomization = -1;
        this.midCustomization = -1;
        this.botCustomization = -1;
    }
}

public class PlayerProfileManager : MonoBehaviour
{
    [SerializeField]
    private ProfileData PlayerProfile;
    public static PlayerProfileManager singleton;
    SaveFileHandler profileHandler;
    void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
        profileHandler = new SaveFileHandler(Application.persistentDataPath, "prof.json");
    }

    void Start(){
    if (!LoadProfile()){
        ModalManager.singleton.SummonModal(1, "PlayerProfile_Intro");
    } 
    }
    public bool LoadProfile(){
        PlayerProfile = profileHandler.Load();
        bool existing = PlayerProfile != null;
        return existing;
    }

    public void SaveProfile(){
        profileHandler.Save(PlayerProfile);
    }

    public void StartNewProfile(ProfileData _profile){
        PlayerProfile = _profile;
    }
    public Color[] GetColors(){
        TextAsset colors = Resources.Load("sProfile/CharColors") as TextAsset;
        List<Color> vals = new List<Color>();
        string[] col = colors.text.Split('\n');
        for(int i = 1; i < col.Length; i++){
            try
            {
                string[] splitVals = col[i].Split(',');
                Color parsed = new Color(int.Parse(splitVals[1])/255.0f, int.Parse(splitVals[2])/255.0f, int.Parse(splitVals[3])/255.0f);
                vals.Add(parsed);
            }
            catch (System.Exception)
            {
                continue;
            }
        }
        return vals.ToArray();
    }
}
