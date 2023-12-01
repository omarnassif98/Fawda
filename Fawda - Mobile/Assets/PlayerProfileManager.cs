using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Text;
using UnityEngine.Events;
using Unity.Mathematics;


public class PlayerProfileManager : MonoBehaviour
{
    [SerializeField]
    private ProfileData PlayerProfile;
    public static PlayerProfileManager singleton;
    SaveFileHandler profileHandler;

    public UnityEvent loadEvent = new UnityEvent(), saveEvent = new UnityEvent(), createEvent = new UnityEvent();

    void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
        profileHandler = new SaveFileHandler(Application.persistentDataPath, "prof.json");
    }

    public void Start(){
        ClientConnection.singleton.RegisterServerEventListener("connect",SendProfilePayload);
    }

    void SendProfilePayload(){
        if (PlayerProfile == null) {
            Debug.LogError("WOAH NO PROFILE AND CONNECT?");
            return;
        }
        byte[] data = PlayerProfile.Encode();
        ClientConnection.singleton.SendMessageToServer(OpCode.PROFILE_PAYLOAD,data);
    }

    public bool LoadProfile(){
        PlayerProfile = profileHandler.Load();
        bool existing = PlayerProfile != null;
        if(existing) loadEvent.Invoke(); 
        else createEvent.Invoke();
        return existing;
    }

    public void SaveProfile(){
        profileHandler.Save(PlayerProfile);
        saveEvent.Invoke();
        loadEvent.Invoke();
    }

    public void StartNewProfile(ProfileData _profile){
        PlayerProfile = _profile;
    }

    public void DeleteProfile(){
        profileHandler.Delete();
        PlayerProfile = null;
    }

    public ProfileData GetProfileData(){
        return PlayerProfile;
    }

    public void SetProfileData(ProfileData _newProfile){
        PlayerProfile = _newProfile;
        SaveProfile();
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
