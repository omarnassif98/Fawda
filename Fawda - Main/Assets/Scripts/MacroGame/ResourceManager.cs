using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NamedColor{
    int idx;
    public string name;
    public Color color;
    public NamedColor(string _csvRow){
        string[] vals = _csvRow.Split(',');
        idx = int.Parse(vals[0]);
        name = vals[1];
        ColorUtility.TryParseHtmlString(vals[2],out color);
    }
}
public class ResourceManager : MonoBehaviour
{
    public static NamedColor[] namedColors {get; private set;}
    void Start(){
        TextAsset tex = Resources.Load("player_colors") as TextAsset;
        string[] rows = tex.text.Split('\n');
        namedColors = new NamedColor[rows.Length];
        for(int i = 0; i < rows.Length; i++){
            namedColors[i] = new NamedColor(rows[i]);
        }
    }
}
