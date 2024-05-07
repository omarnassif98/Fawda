using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIManager))]
public class UITester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIManager singleton = (UIManager)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("Roster Roulette testing");
        GUILayout.Space(5);
        if(GUILayout.Button("Spin")){
            UIManager.RosterManager.StartRoulette();
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



    }
}
