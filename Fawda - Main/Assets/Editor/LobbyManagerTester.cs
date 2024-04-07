using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LobbyManager))]
public class LobbyManagerTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LobbyManager singleton = (LobbyManager)target;
        GUILayout.Space(10);

        if(GUILayout.Button("Add to roster (debug only UI)")){
            singleton.JoinTestPlayerAtFirstAvailable();
        }



    }
}
