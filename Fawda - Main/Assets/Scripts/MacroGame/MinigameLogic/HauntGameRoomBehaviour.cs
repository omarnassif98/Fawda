using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntGameRoomBehaviour : MonoBehaviour
{
    static int num = 0;
    static bool extentReached;
    static float width_used = 0, height_used = 0, view_angle_factor = 1;
    const float FLOOR_THICKNESS = 0.2f, MAX_MAP_HEIGHT = 600, MAX_MAP_WIDTH = 400, OFFSET = FLOOR_THICKNESS + 5.0f/2;
    float targetOpacity = 1;
    List<Renderer> bottomWallMats = new List<Renderer>();
    short occupancy = 0;
    // Start is called before the first frame update


    void Awake(){

        float viewAngle = Camera.main.transform.eulerAngles.x;
        view_angle_factor = 1/Mathf.Sin(Mathf.Deg2Rad * viewAngle);
        print("VIEW ANGLE FACTOR = " + view_angle_factor + " - from: " + viewAngle);
    }
    void FixedUpdate(){
        if(bottomWallMats == null) return;
        targetOpacity = occupancy == 0? 1 : 0.2f;
        foreach(Renderer mat in bottomWallMats){
            mat.material.color = new Color(mat.material.color.r, mat.material.color.g, mat.material.color.b, Mathf.Lerp(mat.material.color.a, targetOpacity, Time.deltaTime / 2));
        }
    }

    public void FeedWalls(List<GameObject> _walls){
        foreach(GameObject wallPart in _walls){
            print(wallPart.name);
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
