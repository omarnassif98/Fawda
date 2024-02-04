using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseGenerator : MonoBehaviour
{
    const float MAX_HEIGHT = 45, MAX_WIDTH = 30;


    void Start(){
        GenerateRoom(0);
        GenerateRoom(1);
    }

    void GenerateRoom(int _forceRand = -1){
        GameObject room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        room.transform.parent = transform;
        room.transform.position = Vector3.zero;
        switch ((_forceRand == -1)?Random.Range(0,2):_forceRand){
            case 0:
                room.transform.localScale = new Vector3(3,0.2f,25);
                break;
            case 1:
                room.transform.localScale = new Vector3(12,0.2f,10);
                break;
        }

    }
}
