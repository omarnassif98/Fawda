using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntNormalPlayerBehaviour : PlayerBehaviour
{
    const float FOV_ANGLES = 30, FOV_MAX = 15;
    const int FOV_RAYS = (int)(FOV_ANGLES * 2.5f);
    MeshFilter playerFOVMesh;

    [SerializeField] Transform lookAtBall;

    void Awake(){
        playerFOVMesh = transform.Find("FOV").GetComponent<MeshFilter>();
        playerFOVMesh.name = "FOV Mesh";
        playerFOVMesh.mesh = new Mesh();
        PlayerBehaviour.hotseat = this;
    }

    protected override void Tick()
    {
        DrawFOV();
        if(PlayerBehaviour.hotseat != this) return;
        Vector2 rotInput = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
        if(rotInput == Vector2.zero) return;
        lookAtBall.position = transform.position + new Vector3(rotInput.x, 0, rotInput.y);
        transform.LookAt(lookAtBall);

    }

    void DrawFOV(){
        Vector3[] arcPoints = new Vector3[FOV_RAYS]; //The FOV Mesh is just a daisy chain of all points
        GameObject lastObj = null; //needed to trip dispute resolution
        for(int i = 0; i < FOV_RAYS; i++){
            float rayAngle = transform.eulerAngles.y - FOV_ANGLES/2 + FOV_ANGLES/FOV_RAYS * i; //fan out the FOV from left to right
            Ray ray = new Ray(transform.position, GetAngleDir(rayAngle));

            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, FOV_MAX)){ // handle hitting an object before FOV limit
                arcPoints[i] = hit.collider.gameObject == lastObj || i == 0?  //if the ray is the first or the object hit is the same as the last ray's, use the point of collision
                 hit.point:SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle, false); //If not basically do a binary search to find the vertex of the first object
                lastObj = hit.collider.gameObject; //Update the last object
            }else if(lastObj == null){
                arcPoints[i] = transform.position + GetAngleDir(rayAngle); //
            }else{
                arcPoints[i] = SettleRayDispute(rayAngle - FOV_ANGLES/FOV_RAYS, rayAngle);
                lastObj = null; //NOT RESETING MAKES RECURSIVE FUNCTION THROW INFINITE LOOP
            }
        }
        Vector3[] meshPoints = new Vector3[FOV_RAYS + 1];
        int[] meshTrianglePointIdxs = new int[(FOV_RAYS - 1) * 3]; //The FOV is drawn as a combination of triangles, think about it
        meshPoints[0] = Vector3.zero;
        for(int i = 0; i<FOV_RAYS; i++){
            meshPoints[i+1] = transform.InverseTransformPoint(arcPoints[i]);
        }

        for(int i = 0; i<FOV_RAYS-1; i++){ //from player to 2 points
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
        float midAngle = (_leftAngle + _rightAngle)/2; //check center
        Ray midRay = new Ray(transform.position, GetAngleDir(midAngle));
        RaycastHit hit;
        if(Physics.Raycast(midRay,out hit, FOV_MAX)){ //if an object is hit
            _biasDist = Vector3.Distance(transform.position, hit.point);
            return _iter >= 3? hit.point: //starting from the third iteration return the collision point
            SettleRayDispute(biasRight?midAngle:_leftAngle, biasRight?_rightAngle:midAngle, biasRight,_iter+1,_biasDist); //if not continue recusively binary search a new iteration
        }else return _iter >= 5? //if no object is hit
         transform.position + GetAngleDir(midAngle,_biasDist): //starting from fifth iteration, 'give up' and return the latest binary search angle at max FOV
         SettleRayDispute(biasRight?_leftAngle:midAngle,biasRight?midAngle:_rightAngle, biasRight, _iter+1,_biasDist); //if not continue recusively binary search a new iteration

    }

    Vector3 GetAngleDir(float _angle, float _dist = -1){
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad),0,Mathf.Cos(_angle * Mathf.Deg2Rad)) * ((_dist == -1 || _dist > FOV_MAX)?FOV_MAX:_dist);
    }
}


