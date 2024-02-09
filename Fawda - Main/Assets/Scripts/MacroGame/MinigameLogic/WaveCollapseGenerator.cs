using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseGenerator : MonoBehaviour
{
    

    void Start(){
        GenerateRoom();
    }

    void GenerateRoom(){
        HauntGameRoomBehaviour room = new GameObject().AddComponent<HauntGameRoomBehaviour>();
        room.transform.parent = transform;
        room.transform.position = Vector3.zero;
        room.Generate();
    }



}
