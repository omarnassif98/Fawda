using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClientConnection), true)]
public class ConnectionTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ClientConnection singleton = (ClientConnection)target;
        GUILayout.Space(10);


        if(GUILayout.Button("Connect")){
            singleton.TriggerServerEvent("connect");
        }
        GUILayout.Space(5);
        if(GUILayout.Button("Connect")){
            //singleton.QueueRPC(new );
        }
        EditorGUILayout.Space(5);
}
}
