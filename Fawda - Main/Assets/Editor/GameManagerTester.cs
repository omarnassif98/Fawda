using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameManager singleton = (GameManager)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("Load Haunt (Staging)");
        GUILayout.Space(5);
        if(GUILayout.Button("Load")){
            singleton.LoadMinigame(GameCodes.HAUNT);
            singleton.ConfigureGame();
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



    }
}
