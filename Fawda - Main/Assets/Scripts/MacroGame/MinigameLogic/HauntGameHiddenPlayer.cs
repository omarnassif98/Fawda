using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameHiddenPlayer : PlayerBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField] float stunDuration, visibilityDuration, killDuration, killRadius;
    bool canKill = true;
    private Coroutine stunCycle;
    private HauntHunterPlayerBehaviour[] hauntHunterPlayers;

    protected override void Tick()
    {
        CheckForKill();
    }

    private Vector2 PullCoord(Vector3 _vec3) => new Vector2(_vec3.x,_vec3.z);
    void CheckForKill(){
        if(!canKill || !isMobile) return;
        foreach(HauntHunterPlayerBehaviour player in hauntHunterPlayers){
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
        canKill = false;
        isMobile = false;
        meshRenderer.enabled = true;
        stunCycle = StartCoroutine(StunRecovery());
    }

    IEnumerator StunRecovery(){
        yield return new WaitForSeconds(stunDuration);
        isMobile = true;
        yield return new WaitForSeconds(visibilityDuration);
        meshRenderer.enabled = false;
        canKill = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        hauntHunterPlayers = GameObject.FindObjectsOfType<HauntHunterPlayerBehaviour>();
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
