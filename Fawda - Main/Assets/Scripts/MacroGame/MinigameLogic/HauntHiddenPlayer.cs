using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntHiddenPlayerBehaviour : PlayerBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField] float stunDuration, visibilityDuration, killDuration, killRadius;
    public bool isStunned {get; private set;}
    private Coroutine stunCycle;
    short health = 7;

    protected override void Tick()
    {
        if(!LobbyManager.gameManager.activeMinigame.gameInPlay) return;
        CheckForKill();
    }

    private Vector2 PullCoord(Vector3 _vec3) => new Vector2(_vec3.x,_vec3.z);
    void CheckForKill(){
        if(isStunned || !isMobile) return;
        foreach(HauntHunterPlayerBehaviour player in ((HauntGameDeployable)LobbyManager.gameManager.activeMinigame).hunterPlayerInstances){
            if(player.isPetrified || Vector2.Distance(PullCoord(player.transform.position), PullCoord(transform.position)) > killRadius) continue;
                print("KILL");
                StartCoroutine(PetrifyHunter(player));
        }
    }

    private IEnumerator PetrifyHunter(HauntHunterPlayerBehaviour _victim){
        isMobile = false;
        transform.LookAt(new Vector3(_victim.transform.position.x, transform.position.y, _victim.transform.position.z));
        meshRenderer.enabled = true;
        _victim.Petrify();
        yield return new WaitForSeconds(killDuration);
        meshRenderer.enabled = false;
        isMobile = true;
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
        yield return new WaitForSeconds(stunDuration);
        isMobile = true;
        yield return new WaitForSeconds(visibilityDuration);
        meshRenderer.enabled = false;
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
