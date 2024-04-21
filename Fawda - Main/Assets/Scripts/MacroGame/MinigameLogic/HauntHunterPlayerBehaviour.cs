using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntHunterPlayerBehaviour : PlayerBehaviour
{

    public bool isPetrified{ get; private set;}
    public bool isInvincible{ get; private set;}
    private bool wasNearGhostLastFrame = false;
    const float RELOAD_TIME = 1.25f;
    float reloadProgress = 0;
    [SerializeField] Transform lookAtBall;
    HauntHunterFOVHelper fovHelper;
    const short MAX_AMMO = 2;
    private short currentAmmo = MAX_AMMO;


    Material playerPetrifiedMaterial, playerStressedMaterial, playerReloadingMaterial;
    protected override void Awake(){
        fovHelper = new HauntHunterFOVHelper(transform.Find("FOV").GetComponent<MeshFilter>(), this);
        PlayerBehaviour.hotseat = this;
        isPetrified = false;
        playerPetrifiedMaterial = Resources.Load("MinigameAssets/Haunt/Materials/PetifiedPlayerMat") as Material;
        playerStressedMaterial = Resources.Load("MinigameAssets/Haunt/Materials/StressedPlayerMat") as Material;
        playerReloadingMaterial = Resources.Load("MinigameAssets/Haunt/Materials/ReloadPlayerMat") as Material;

        base.Awake();
    }

    protected override void Tick()
    {
        if(PlayerBehaviour.hotseat != this || !GameManager.activeMinigame.gameInPlay || isPetrified) return;
        Vector2 rotInput = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
        if(Input.GetButton("Alt Action") && currentAmmo < MAX_AMMO){
            isMobile = false;
            playerRenderer.material = playerReloadingMaterial;
            reloadProgress += Time.deltaTime;
            if(reloadProgress < RELOAD_TIME) return;
            reloadProgress = 0;
            currentAmmo += 1;
            FlushStressMaterial();
            isMobile = true;
        }

        if(Input.GetButtonUp("Alt Action")){
            FlushStressMaterial();
            isMobile = true;
        }

        if(!isMobile) return;
        if(Input.GetButtonDown("Action") && currentAmmo > 0){
            currentAmmo -= 1;
            isMobile = false;
            StartCoroutine(fovHelper.FlashFOV(() => {isMobile = true;}));
        }
        UpdateStressCondition();
        if(rotInput == Vector2.zero) return;
        lookAtBall.position = transform.position + new Vector3(rotInput.x, 0, rotInput.y);
        transform.LookAt(lookAtBall);
    }

    void UpdateStressCondition(){
        bool isNearGhost = Vector3.Distance(transform.position, ((HauntGameDeployable)GameManager.activeMinigame).ghostPlayerInstance.transform.position) < 5;
        if(isNearGhost != wasNearGhostLastFrame) HandleStressShift(isNearGhost);
        wasNearGhostLastFrame = isNearGhost;
        if(!isNearGhost) return;
        //Heartbeat effect goes here
    }


    void HandleStressShift(bool _isNearGhost){
        DebugLogger.SourcedPrint(gameObject.name, "Stressed " + _isNearGhost.ToString());
        Material newPlayerMaterial = _isNearGhost?playerStressedMaterial:playerDefaultMaterial;
        playerRenderer.material = newPlayerMaterial;
    }

    void FlushStressMaterial(){
        DebugLogger.SourcedPrint(gameObject.name, "Flushing stress");
        Material newPlayerMaterial = Vector3.Distance(transform.position, ((HauntGameDeployable)GameManager.activeMinigame).ghostPlayerInstance.transform.position) < 5?playerStressedMaterial:playerDefaultMaterial;
        playerRenderer.material = newPlayerMaterial;
    }



    public void Petrify(){
        print("Petrified");
        isPetrified = true;
        isMobile = false;
        //Tell game manager to zoom in ig
        playerRenderer.material = playerPetrifiedMaterial;
    }

    public IEnumerator Revive(){
        isPetrified = false;
        isMobile = true;
        isInvincible = true;
        playerRenderer.material = playerDefaultMaterial;
        yield return new WaitForSeconds(0.05f);
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(0.05f);
        playerRenderer.enabled = true;
        yield return new WaitForSeconds(0.13f);
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(0.05f);
        playerRenderer.enabled = true;
        yield return new WaitForSeconds(0.13f);
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(0.05f);
        playerRenderer.enabled = true;
        isInvincible = false;

    }

    public void DrawFOV(Vector3[] _meshPoints, int[] _trianglePoints){

    }
}


