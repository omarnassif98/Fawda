using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour hotseat; //Debug var
    protected bool isMobile = false;
    const float speed =  4.5f;
    protected Material playerDefaultMaterial;
    protected Renderer playerRenderer;
    private ParticleSystem smokeEmitter;
    private int idx;

    protected virtual void Awake(){
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerRenderer = transform.Find("PlayerRenderer").GetComponent<Renderer>();
        smokeEmitter = transform.Find("Smoke Particles").GetComponent<ParticleSystem>();
    }

    public void Initialize(int _idx){
        idx = _idx;
        isMobile = true;
    }

    public void PoofPlayer(bool _activityStatus){
        playerRenderer.enabled = _activityStatus;
        smokeEmitter.Clear();
        smokeEmitter.Play();
        isMobile = _activityStatus;
    }

    void Update(){
        if (isMobile)
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        JoypadState joypadState = InputManager.joypadStates[idx];
        transform.position += new Vector3(joypadState.analog.x, 0, joypadState.analog.y) * Time.deltaTime * speed;
    }

    protected abstract void Tick();

}