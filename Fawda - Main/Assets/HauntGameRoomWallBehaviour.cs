using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameRoomWallBehaviour : MonoBehaviour
{
    float targetOpacity = 1;
    Renderer[] wallMats = null;
    short occupancy = 0;
    // Start is called before the first frame update
    void Awake()
    {
        wallMats = transform.GetComponentsInChildren<Renderer>();    
    }

    void FixedUpdate(){
        targetOpacity = occupancy == 0? 1 : 0.2f;


        foreach(Renderer mat in wallMats){
            mat.material.color = new Color(mat.material.color.r, mat.material.color.g, mat.material.color.b, Mathf.Lerp(mat.material.color.a, targetOpacity, Time.deltaTime));
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
