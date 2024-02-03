using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntNormalPlayerBehaviour : PlayerBehaviour
{
    const float FOV_ANGLES = 35;
    const int FOV_RAYS = 20;
    protected override void Tick()
    {
        print("Tik");
        DrawFOV();
    }

    void DrawFOV(){
        Vector3[] arcPoints = new Vector3[FOV_RAYS];
        GameObject lastObj = null;
        for(int i = 0; i < FOV_RAYS; i++){
            float rayAngle = transform.eulerAngles.y - FOV_ANGLES/2 + FOV_ANGLES/FOV_RAYS * i;
            Ray ray = new Ray(transform.position, GetAngleDir(rayAngle));
            
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit)){
                arcPoints[i] = hit.collider.gameObject == lastObj || i == 0? hit.point:SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle, lastObj, false);
                lastObj = hit.collider.gameObject;
            }else if(lastObj == null){
                arcPoints[i] = transform.position + GetAngleDir(rayAngle);
            }else{
                arcPoints[i] = SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle, lastObj);
                lastObj = null; //NOT RESETING MAKES RECURSIVE FUNCTION THROW INFINITE LOOP
            }

            Debug.DrawLine(transform.position, arcPoints[i], Color.red);
        } 
    }

    Vector3 SettleRayDispute(float _leftAngle, float _rightAngle, GameObject _matchObj, bool biasRight = true, int _iter = 1){
        float midAngle = (_leftAngle + _rightAngle)/2;
        Ray midRay = new Ray(transform.position, GetAngleDir(midAngle));
        RaycastHit hit;
        if(Physics.Raycast(midRay,out hit)){
            return _iter >= 6? hit.point:SettleRayDispute(biasRight?midAngle:_leftAngle, biasRight?_rightAngle:midAngle,_matchObj,biasRight,_iter+1);
        }else{
            return SettleRayDispute(biasRight?_leftAngle:midAngle, biasRight?midAngle:_rightAngle, _matchObj, biasRight, _iter+1);
        }
    }

    Vector3 GetAngleDir(float _angle){
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad),0,Mathf.Cos(_angle * Mathf.Deg2Rad)) * 25;
    }
}


