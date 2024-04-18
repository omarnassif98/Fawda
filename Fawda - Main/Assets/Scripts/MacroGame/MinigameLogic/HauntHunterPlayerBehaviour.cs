using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntHunterPlayerBehaviour : PlayerBehaviour
{
    const float FOV_ANGLES = 40, FOV_MAX = 200;
    const int FOV_RAYS = (int)(FOV_ANGLES * 1.5f);
    public bool isPetrified{ get; private set;}
    public bool isInvincible{ get; private set;}
    MeshFilter playerFOVMesh;
    [SerializeField] Transform lookAtBall;
    [SerializeField] float flashDuration;

    Material playerPetrifiedMaterial;
    void Awake(){
        playerFOVMesh = transform.Find("FOV").GetComponent<MeshFilter>();
        playerFOVMesh.name = "FOV Mesh";
        playerFOVMesh.mesh = new Mesh();
        PlayerBehaviour.hotseat = this;
        isPetrified = false;
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerPetrifiedMaterial = Resources.Load("MinigameAssets/Haunt/Materials/PetifiedPlayerMat") as Material;
    }

    protected override void Tick()
    {
        if(PlayerBehaviour.hotseat != this || !base.isMobile || !GameManager.activeMinigame.gameInPlay) return;
        Vector2 rotInput = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
        if(Input.GetButtonDown("Action")) StartCoroutine(FlashCamera());
        if(rotInput == Vector2.zero) return;
        lookAtBall.position = transform.position + new Vector3(rotInput.x, 0, rotInput.y);
        transform.LookAt(lookAtBall);

    }

    IEnumerator FlashCamera(){
        base.isMobile = false;
        playerFOVMesh.gameObject.SetActive(true);
        DrawFOV();
        if(Vector3.Angle(transform.forward, ((HauntGameDeployable)(GameManager.activeMinigame)).ghostPlayerInstance.transform.position - transform.position) < FOV_ANGLES) ((HauntGameDeployable)(GameManager.activeMinigame)).ghostPlayerInstance.Stun();

        foreach(HauntHunterPlayerBehaviour hauntHunter in ((HauntGameDeployable)(GameManager.activeMinigame)).hunterPlayerInstances){
            if(hauntHunter == this || !hauntHunter.isPetrified) continue;
            if(Vector3.Angle(transform.forward, hauntHunter.transform.position - transform.position) < FOV_ANGLES) hauntHunter.StartCoroutine(hauntHunter.Revive());
        }
        yield return new WaitForSeconds(flashDuration);
        base.isMobile = true;
        playerFOVMesh.gameObject.SetActive(false);

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

    public void Petrify(){
        print("Petrified");
        isPetrified = true;
        isMobile = false;
        //Tell game manager to zoom in ig
        GetComponent<Renderer>().material = playerPetrifiedMaterial;
    }

    public IEnumerator Revive(){
        isPetrified = false;
        isMobile = true;
        isInvincible = true;
        GetComponent<Renderer>().material = playerDefaultMaterial;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.13f);
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(0.13f);
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().enabled = true;
        isInvincible = false;
    }
}


