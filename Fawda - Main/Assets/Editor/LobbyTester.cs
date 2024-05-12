using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LobbyManager))]
public class LobbyTester : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LobbyManager singleton = (LobbyManager)target;
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("helpbox");
        GUILayout.Space(2);
        GUILayout.Label("Roster Roulette testing");
        GUILayout.Space(5);
        if(GUILayout.Button("FastTrack")){
            if(LobbyManager.players[0] == null){
                ConnectionManager.singleton.HandlePlayerConnect(0);
                NetMessage msg = new NetMessage(OpCode.PROFILE_PAYLOAD, new ProfileData("Omar", 0).Encode());
                ConnectionManager.singleton.QueueRPC(new DirectedNetMessage(msg,0));
            }
            LobbyManager.gameManager.LoadMinigame(GameCodes.HAUNT);
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
}
