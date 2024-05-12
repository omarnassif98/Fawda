using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorButtonBehaviour), true)]
public class FloorButtonTester : Editor
{
    bool vis = true;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FloorButtonBehaviour singleton = (FloorButtonBehaviour)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");

        if(GUILayout.Button("Toggle visibility")){
            vis = !vis;
            singleton.SetVisibility(vis);

        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



    }
}
