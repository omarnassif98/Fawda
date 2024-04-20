using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HauntHunterPlayerBehaviour : PlayerBehaviour
{

    public bool isPetrified{ get; private set;}
    public bool isInvincible{ get; private set;}
    private short sanity = 100;
    [SerializeField] Transform lookAtBall;
    IEnumerable currentlyRunningSanityLoop;
    HauntHunterFOVHelper fovHelper;



    Material playerPetrifiedMaterial, playerStressedMaterial;
    void Awake(){
        fovHelper = new HauntHunterFOVHelper(transform.Find("FOV").GetComponent<MeshFilter>(), this);
        PlayerBehaviour.hotseat = this;
        isPetrified = false;
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerPetrifiedMaterial = Resources.Load("MinigameAssets/Haunt/Materials/PetifiedPlayerMat") as Material;
        playerStressedMaterial = Resources.Load("MinigameAssets/Haunt/Materials/StressedPlayerMat") as Material;
        isMobile = true;
    }

    protected override void Tick()
    {
        if(PlayerBehaviour.hotseat != this || !base.isMobile || !GameManager.activeMinigame.gameInPlay) return;
        Vector2 rotInput = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
        if(Input.GetButtonDown("Action")){
            isMobile = false;
            StartCoroutine(fovHelper.FlashCamera(() => {isMobile = true;}));
        }
        if(rotInput == Vector2.zero) return;
        lookAtBall.position = transform.position + new Vector3(rotInput.x, 0, rotInput.y);
        transform.LookAt(lookAtBall);
    }

    IEnumerator SanityLoop(){
        yield return new WaitForSeconds(0);
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
        sanity = 50;
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

    public void DrawFOV(Vector3[] _meshPoints, int[] _trianglePoints){

    }
}


