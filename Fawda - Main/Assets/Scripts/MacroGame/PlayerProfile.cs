using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct PlayerProfile
{
    int connectionIdx;
    string playerName;
    Color playerColor;
    public PlayerProfile(int _connectionIdx, byte[] _rawData){
        connectionIdx = _connectionIdx;
        int colorIdx = BitConverter.ToInt16(_rawData,0);
        playerName = BitConverter.ToString(_rawData,1);
        ColorUtility.TryParseHtmlString("#black", out playerColor);
    }
}
 
