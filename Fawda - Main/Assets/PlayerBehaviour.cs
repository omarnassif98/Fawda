using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour hotseat; //Debug var
    protected bool isMobile = true;
    const float speed =  4.5f;
    protected Material playerDefaultMaterial;
    protected Renderer playerRenderer;
    private ParticleSystem smokeEmitter;

    protected virtual void Awake(){
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerRenderer = transform.Find("PlayerRenderer").GetComponent<Renderer>();
        smokeEmitter = transform.Find("Smoke Particles").GetComponent<ParticleSystem>();
    }

    public void EmitSmoke(){
        playerRenderer.enabled = false;
        smokeEmitter.Clear();
        smokeEmitter.Play();
        isMobile = false;
    }

    void Update(){
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        if(hotseat != this || !isMobile) return;
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * speed;
    }

    protected abstract void Tick();

}