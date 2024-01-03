using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursorManager : MonoBehaviour
{
    public static MenuCursorManager singleton;
    private MenuCursorBehaviour[] cursors = new MenuCursorBehaviour[SynapseServer.MAX_PLAYERS];
    void Awake(){
        if(singleton != null) return;
        singleton = this;
        for(short i = 0; i < cursors.Length; i++){
            cursors[i] = GameObject.Find("Cursors").transform.GetChild(i).GetComponent<MenuCursorBehaviour>();
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
