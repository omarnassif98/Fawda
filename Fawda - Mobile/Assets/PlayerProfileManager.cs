using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class PlayerProfileManager : MonoBehaviour
{
    private string playerName;
    private short playerColor;
    public static PlayerProfileManager singleton;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetPlayerColor(short _playerColor){
        playerColor = _playerColor;
    }

    public void SetPlayerName(string _playerName){
        playerName = _playerName;
    }

    public byte[] GetProfilePayload(){
        byte[] nameBytes = Encoding.ASCII.GetBytes(playerName);
        
        NetMessage payload = new NetMessage(OpCode.PROFILE_PAYLOAD, nameBytes);
        return null;
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
