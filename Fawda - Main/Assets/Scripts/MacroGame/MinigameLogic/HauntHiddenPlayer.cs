using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntHiddenPlayerBehaviour : PlayerBehaviour
{
    public Renderer meshRenderer;
    const float STUN_DURATION = 0.75f, STUN_BLINK = 0.15f, VISIBILITY_DURATION = 1, KILL_DURATION = 1.25f, KILL_RADIUS = 1.2f;
    public bool isStunned {get; private set;}
    bool canSprint = true;
    private Coroutine stunCycle;
    short health = 7;
    float sprintSpeed = 6;
    Color hurtColor = Color.red, normalColor = Color.gray, scareColor = Color.cyan;
    public override void Tick()
    {

        if (!isMobile) return;
        Move();
        if (canSprint) Sprint();
        CheckForKill();
    }


    void CheckForKill(){
        if(isStunned || !isMobile) return;
        foreach(PlayerBehaviour player in ((HauntGameDeployable)LobbyManager.gameManager.activeMinigame).playerInstances){
            if (player is HauntHiddenPlayerBehaviour) continue;
            HauntHunterPlayerBehaviour hauntPlayer = (HauntHunterPlayerBehaviour)player;
            if (hauntPlayer.isPetrified || Vector3.Distance(player.transform.position, transform.position) > KILL_RADIUS) continue;
            print("KILL");
            StartCoroutine(PetrifyHunter(hauntPlayer));
        }
    }



    private IEnumerator PetrifyHunter(HauntHunterPlayerBehaviour _victim){
        isMobile = false;
        transform.LookAt(new Vector3(_victim.transform.position.x, transform.position.y, _victim.transform.position.z));
        meshRenderer.enabled = true;
        meshRenderer.material.color = scareColor;
        _victim.Petrify();
        yield return new WaitForSeconds(KILL_DURATION);
        isMobile = true;
        meshRenderer.material.color = normalColor;
        meshRenderer.enabled = false;
        DioramaControllerBehaviour.singleton.ClearTrackTransform();
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
        meshRenderer.material.color = hurtColor;
        yield return new WaitForSeconds(STUN_DURATION);

        isMobile = true;
        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = true;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = true;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.enabled = true;

        yield return new WaitForSeconds(STUN_BLINK);
        meshRenderer.material.color = normalColor;


        yield return new WaitForSeconds(VISIBILITY_DURATION);
        meshRenderer.enabled = false;

        canSprint = true;
        isStunned = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        isStunned = false;
        meshRenderer = transform.Find("PlayerRenderer").GetComponent<Renderer>();
        meshRenderer.enabled = false;
        meshRenderer.material.color = normalColor;
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
