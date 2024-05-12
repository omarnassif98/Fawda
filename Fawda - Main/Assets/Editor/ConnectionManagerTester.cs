using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConnectionManager), true)]
public class ConnectionManagerTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ConnectionManager singleton = (ConnectionManager)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");

        if(GUILayout.Button("Add New User")){
            for(int i = 0; i < LobbyManager.players.Length; i++){
                if(LobbyManager.players[i] != null) continue;
                ConnectionManager.singleton.HandlePlayerConnect(i);
                NetMessage msg = new NetMessage(OpCode.PROFILE_PAYLOAD, new ProfileData("Omar", i).Encode());
                singleton.QueueRPC(new DirectedNetMessage(msg,i));
                return;
                }
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginVertical("helpbox");

        if(GUILayout.Button("Ready player 1")){
            NetMessage netMessage = new NetMessage(OpCode.READYUP,new PlayerGameReadyUpData(true).Encode());
            DirectedNetMessage directedNetMessage = new DirectedNetMessage(netMessage, 0);
            ConnectionManager.singleton.QueueRPC(directedNetMessage);
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



    }
}
