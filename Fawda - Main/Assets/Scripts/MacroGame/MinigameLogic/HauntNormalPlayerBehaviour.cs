using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntNormalPlayerBehaviour : PlayerBehaviour
{
    const float FOV_ANGLES = 45, FOV_MAX = 400;
    const int FOV_RAYS = (int)(FOV_ANGLES * 2.5f);
    MeshFilter playerFOVMesh;

    void Awake(){
        playerFOVMesh = transform.Find("FOV").GetComponent<MeshFilter>();
        playerFOVMesh.name = "FOV Mesh";
        playerFOVMesh.mesh = new Mesh();
    }

    protected override void Tick()
    {
        
    }

    void LateUpdate(){
        DrawFOV();
    }

    void DrawFOV(){
        Vector3[] arcPoints = new Vector3[FOV_RAYS];
        GameObject lastObj = null;
        for(int i = 0; i < FOV_RAYS; i++){
            float rayAngle = transform.eulerAngles.y - FOV_ANGLES/2 + FOV_ANGLES/FOV_RAYS * i;
            Ray ray = new Ray(transform.position, GetAngleDir(rayAngle));
            
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, FOV_MAX)){
                arcPoints[i] = hit.collider.gameObject == lastObj || i == 0? hit.point:SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle, false);
                lastObj = hit.collider.gameObject;
            }else if(lastObj == null){
                arcPoints[i] = transform.position + GetAngleDir(rayAngle);
            }else{
                arcPoints[i] = SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle);
                lastObj = null; //NOT RESETING MAKES RECURSIVE FUNCTION THROW INFINITE LOOP
            }

            Debug.DrawLine(transform.position, arcPoints[i], Color.red);
        } 
        Vector3[] meshPoints = new Vector3[FOV_RAYS + 1];
        int[] meshTrianglePointIdxs = new int[(FOV_RAYS - 1) * 3];
        meshPoints[0] = Vector3.zero;
        for(int i = 0; i<FOV_RAYS; i++){
            meshPoints[i+1] = transform.InverseTransformPoint(arcPoints[i]);
        }

        for(int i = 0; i<FOV_RAYS-1; i++){
            meshTrianglePointIdxs[i * 3] = 0;
            meshTrianglePointIdxs[i * 3 + 1] = i + 1;
            meshTrianglePointIdxs[i * 3 + 2] = i + 2;
        }
        playerFOVMesh.mesh.Clear();
        playerFOVMesh.mesh.vertices = meshPoints;
        playerFOVMesh.mesh.triangles = meshTrianglePointIdxs;
        playerFOVMesh.mesh.RecalculateNormals();

    }

    Vector3 SettleRayDispute(float _leftAngle, float _rightAngle, bool biasRight = true, int _iter = 1, float _biasDist = -1){
        float midAngle = (_leftAngle + _rightAngle)/2;
        Ray midRay = new Ray(transform.position, GetAngleDir(midAngle));
        RaycastHit hit;
        if(Physics.Raycast(midRay,out hit)){
            _biasDist = Vector3.Distance(transform.position, hit.point);
            return _iter >= 3? hit.point:SettleRayDispute(biasRight?midAngle:_leftAngle, biasRight?_rightAngle:midAngle, biasRight,_iter+1,_biasDist);
        }else{
            return _iter >= 5? transform.position + GetAngleDir(midAngle,_biasDist):SettleRayDispute(biasRight?_leftAngle:midAngle, biasRight?midAngle:_rightAngle, biasRight, _iter+1,_biasDist);
        }
    }

    Vector3 GetAngleDir(float _angle, float _dist = -1){
        if (_dist < -1) print("BIAS DIST: " + _dist);
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad),0,Mathf.Cos(_angle * Mathf.Deg2Rad)) * ((_dist == -1 || _dist > FOV_MAX)?FOV_MAX:_dist);
    }
}


