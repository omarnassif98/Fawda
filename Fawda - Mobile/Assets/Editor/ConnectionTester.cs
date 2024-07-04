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
        if(GUILayout.Button("Progress Haunt Game")){
            NetMessage msg = new NetMessage(OpCode.GAMESETUP,new byte[]{(byte)GameCodes.HAUNT});
            singleton.QueueRPC(msg);
        }

        EditorGUILayout.Space(5);
        if(GUILayout.Button("Get Prompt")){
            NetMessage msg = new NetMessage(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(true).Encode());
            singleton.QueueRPC(msg);
        }

        EditorGUILayout.Space(5);
        if(GUILayout.Button("Get Readyup (true)")){
            NetMessage msg = new NetMessage(OpCode.READYUP,new SimpleBooleanMessage(true).Encode());
            singleton.QueueRPC(msg);
        }

        EditorGUILayout.Space(5);
        if(GUILayout.Button("Get Readyup (false)")){
            NetMessage msg = new NetMessage(OpCode.READYUP,new SimpleBooleanMessage(false).Encode());
            singleton.QueueRPC(msg);
        }


    }
}
