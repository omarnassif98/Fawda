using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HauntGameSetupBehaviour))]
public class HauntSetupTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HauntGameSetupBehaviour singleton = (HauntGameSetupBehaviour)target;
        GUILayout.Space(10);

        if(GUILayout.Button("Lock in game")){
            NetMessage msg = new NetMessage(OpCode.READYUP, new byte[]{BitConverter.GetBytes(true)[0],BitConverter.GetBytes(true)[0]});
            for(int i = 0; i < LobbyManager.players.Length; i++) if(LobbyManager.players[i] != null) ConnectionManager.singleton.QueueRPC(new DirectedNetMessage(msg,i));
            ConnectionManager.singleton.FlushRPCQueue(); // Thread safety reasons
            singleton.ReadyUp();
        }



    }
}
