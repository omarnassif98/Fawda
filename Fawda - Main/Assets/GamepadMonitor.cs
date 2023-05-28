using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadMonitor : MonoBehaviour
{

    [SerializeField]
    GameObject gamepadVizPrefab;
    
    public void PopulateVisualizations(){
        short[] idxs = ConnectionManager.singleton.GetPlayerIdxs();
        for(short i = 0; i<idxs.Length; i++){
            GameObject viz = GameObject.Instantiate(gamepadVizPrefab,Vector3.zero,Quaternion.identity,transform);
            viz.GetComponent<DebugGamepadVisualization>().Initialize(idxs[i]);
        }
    }

    public void KillVisualizations(){
        for(short i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
