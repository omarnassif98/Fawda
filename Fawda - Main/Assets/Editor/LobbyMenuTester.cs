using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIManager))]
public class LobbyMenuTester : Editor
{
    Color[] playerColors = new Color[]{new Color(1,1,1,0.4f),new Color(1,0,1,0.4f),new Color(1,0,0,0.4f),new Color(0,0,1,0.4f),new Color(1,1,0,0.4f),new Color(0,0,1,0.4f),new Color(0,1,1,0.4f)};
    string[] playerNames = new string[]{"Fadi","Karam","Toutii","Sara","Omzoooz","Joseph","Chloe","Ali","Riad","Abed"};
    int idx;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIManager singleton = (UIManager)target;
        GUILayout.Space(10);

        if(GUILayout.Button("Add to roster (debug only UI)")){
            UIManager.RosterManager.AddPlayerToRoster(playerNames[Random.Range(0,playerNames.Length)], playerColors[Random.Range(0,playerColors.Length)]);
        }

        GUILayout.Space(10);


        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("Remove from roster (debug only UI)");
        GUILayout.Space(5);
        idx = EditorGUILayout.IntField("idx", idx);
        if(GUILayout.Button("Remove Player")){
            UIManager.RosterManager.RemovePlayerFromRoster((short)idx);
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

        
        
        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("Roullete tester");
        GUILayout.Space(5);
        
        if(GUILayout.Button("Start")){
            UIManager.RosterManager.StartReadyupProcess(new bool[]{true,false,true,false,true});
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

        
    }
}
