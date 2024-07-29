using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour hotseat; //Debug var
    protected bool isMobile = false;
    public bool puppetMode = false;
    protected float baseSpeed =  4.5f, speed;
    protected Material playerDefaultMaterial;
    protected Renderer playerRenderer;
    private ParticleSystem smokeEmitter;
    public int idx { get; private set; }
    public JoypadState currentJoypadState = JoypadState.NEUTRAL;

    protected virtual void Awake(){
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerRenderer = transform.Find("PlayerRenderer").GetComponent<Renderer>();
        smokeEmitter = transform.Find("Smoke Particles").GetComponent<ParticleSystem>();
        speed = baseSpeed;
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
        if (!puppetMode) currentJoypadState = isMobile?InputManager.joypadStates[idx]:JoypadState.NEUTRAL;
        Tick();
    }
    // Update is called once per frame

    protected virtual void Move() => transform.position += new Vector3(currentJoypadState.analog.x, 0, currentJoypadState.analog.y) * Time.deltaTime * speed;
    

    public abstract void Tick();

}