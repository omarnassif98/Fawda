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
    private GameObject smokeEmitterPrefab;
    public int idx { get; private set; }
    public JoypadState currentJoypadState = JoypadState.NEUTRAL;

    protected virtual void Awake(){
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        playerRenderer = transform.Find("PlayerRenderer").GetComponent<Renderer>();
        smokeEmitterPrefab = Resources.Load("Global/Prefabs/Smoke Particles") as GameObject;
        speed = baseSpeed;
        PoofPlayer(true);
    }

    public void Initialize(int _idx){
        idx = _idx;
        isMobile = true;
    }

    public virtual void PoofPlayer(bool _activityStatus){
        playerRenderer.enabled = _activityStatus;
        GameObject.Instantiate(smokeEmitterPrefab, transform.position, Quaternion.identity);
        isMobile = _activityStatus;
    }

    void Update(){
        if (!puppetMode) currentJoypadState = isMobile?InputManager.joypadStates[idx]:JoypadState.NEUTRAL;
        Tick();
    }
    // Update is called once per frame

    protected virtual void Move() => transform.position += new Vector3(currentJoypadState.analog.x, 0, currentJoypadState.analog.y) * Time.deltaTime * speed;
    
    public void Terminate()
    {
        PoofPlayer(false);
        Destroy(gameObject);
    }

    public abstract void Tick();

}