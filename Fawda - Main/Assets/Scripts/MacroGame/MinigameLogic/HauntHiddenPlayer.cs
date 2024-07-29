using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntHiddenPlayerBehaviour : PlayerBehaviour
{
    public MeshRenderer meshRenderer;
    [SerializeField] float stunDuration, visibilityDuration, killDuration, killRadius;
    public bool isStunned {get; private set;}
    bool canSprint = true;
    private Coroutine stunCycle;
    short health = 7;
    float sprintSpeed = 6;

    public override void Tick()
    {
        Move();
        if (canSprint) Sprint();
        if(!LobbyManager.gameManager.activeMinigame.gameInPlay) return;
        CheckForKill();
    }


    void CheckForKill(){
        if(isStunned || !isMobile) return;
        foreach(PlayerBehaviour player in ((HauntGameDeployable)LobbyManager.gameManager.activeMinigame).playerInstances){
            if (player is HauntHiddenPlayerBehaviour) continue;
            HauntHunterPlayerBehaviour hauntPlayer = (HauntHunterPlayerBehaviour)player;
            if (hauntPlayer.isPetrified || Vector3.Distance(player.transform.position, transform.position) > killRadius) continue;
                print("KILL");
                StartCoroutine(PetrifyHunter(hauntPlayer));
        }
    }



    private IEnumerator PetrifyHunter(HauntHunterPlayerBehaviour _victim){
        isMobile = false;
        transform.LookAt(new Vector3(_victim.transform.position.x, transform.position.y, _victim.transform.position.z));
        meshRenderer.enabled = true;
        _victim.Petrify();
        yield return new WaitForSeconds(killDuration);
        isMobile = true;
        meshRenderer.enabled = false;
    }

    public void Sprint()
    {
        speed = currentJoypadState.action ? sprintSpeed : baseSpeed;
        meshRenderer.enabled = currentJoypadState.action;
    }

    public void Stun(){
        if(stunCycle != null) StopCoroutine(stunCycle);
        stunCycle = null;
        isStunned = true;
        isMobile = false;
        meshRenderer.enabled = true;
        stunCycle = StartCoroutine(StunRecovery());
    }

    IEnumerator StunRecovery(){
        canSprint = false;
        yield return new WaitForSeconds(stunDuration);
        isMobile = true;
        yield return new WaitForSeconds(visibilityDuration);
        meshRenderer.enabled = false;
        canSprint = true;
        isStunned = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        meshRenderer = transform.Find("PlayerRenderer").GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        #if UNITY_EDITOR
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.transform.parent = transform;
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localScale = Vector3.one * 0.25f;
            indicator.name = "Indicator";
            Destroy(indicator.gameObject.GetComponent<SphereCollider>());
        #endif
    }

}
