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
            singleton.TestingReadyup();
        }



    }
}
