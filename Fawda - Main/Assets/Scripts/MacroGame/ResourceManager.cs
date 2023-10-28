using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NamedColor{
    int idx;
    string name;
    string hexCode;
    public NamedColor(string _csvRow){
        string[] vals = _csvRow.Split(',');
        idx = int.Parse(vals[0]);
        name = vals[1];
        hexCode = vals[2];
    }
}
public abstract class ResourceManager
{
    public static NamedColor[] GetColors(){
        TextAsset tex = Resources.Load<TextAsset>("player_colors");
        string[] rows = tex.text.Split('\n');
        NamedColor[] res = new NamedColor[rows.Length];
        for(int i = 0; i < rows.Length; i++){
            res[i] = new NamedColor(rows[i]);         
        }
        return res;
    }
}
