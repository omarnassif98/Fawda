using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntGameRoomBehaviour : MonoBehaviour
{
    const float FLOOR_THICKNESS = 0.2f;
    float targetOpacity = 1;
    List<Renderer> bottomWallMats = new List<Renderer>();
    short occupancy = 0;

    void FixedUpdate(){
        if(bottomWallMats == null) return;
        targetOpacity = occupancy == 0? 1 : 0.2f;
        foreach(Renderer mat in bottomWallMats){
            mat.material.color = new Color(mat.material.color.r, mat.material.color.g, mat.material.color.b, Mathf.Lerp(mat.material.color.a, targetOpacity, Time.deltaTime / 2));
        }
    }

    public void FeedWalls(List<GameObject> _walls){
        foreach(GameObject wallPart in _walls){
            bottomWallMats.Add(wallPart.GetComponent<MeshRenderer>());
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider _col){
        occupancy += 1;
    }

    void OnTriggerExit(Collider _col){
        occupancy -= 1;
    }

}
