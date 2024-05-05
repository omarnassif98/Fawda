using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    [Serializable]
    struct camPos{
        public Vector3 pos;
        public Vector3 eulerAngles;
        public camPos(Vector3 _pos, Vector3 _eulerAngles){
            this.pos = _pos;
            this.eulerAngles = _eulerAngles;
        }
    }
    int idealIdx = 0;
    [SerializeField] camPos[] camPosArray;
    void Start()
    {
        ConnectionManager.singleton.RegisterServerEventListener("wakeup",() => {UpdateCameraPosition(1);});
    }

    void UpdateCameraPosition(int _newIdx){
        idealIdx = _newIdx;
        DebugLogger.SourcedPrint("Cam","MOVING","005500");
    }

    void Update(){
        if (Vector3.Distance(camPosArray[idealIdx].pos, transform.position) < 0.15f){
            transform.position = camPosArray[idealIdx].pos;
            transform.eulerAngles = camPosArray[idealIdx].eulerAngles;
            return;
        }
        transform.position = Vector3.Lerp(transform.position, camPosArray[idealIdx].pos, 0.1f);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, camPosArray[idealIdx].eulerAngles, 0.1f);

    }

}
