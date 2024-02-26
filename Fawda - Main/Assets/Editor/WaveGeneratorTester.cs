using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveCollapseGenerator))]
public class WaveGeneratorTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WaveCollapseGenerator singleton = (WaveCollapseGenerator)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("3X3 bento box");
        GUILayout.Space(5);
        if(GUILayout.Button("Regenerate")){
            singleton.GenerateFloormap();
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

        
        
    }
}
