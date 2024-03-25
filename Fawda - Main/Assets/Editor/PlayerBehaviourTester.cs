using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerBehaviour), true)]
public class PlayerBehaviourTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerBehaviour singleton = (PlayerBehaviour)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        if(GUILayout.Button("Hotswitch to this player")){
            PlayerBehaviour.hotseat = singleton;
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



    }
}
