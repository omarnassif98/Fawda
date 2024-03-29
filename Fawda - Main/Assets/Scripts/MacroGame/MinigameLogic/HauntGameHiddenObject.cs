using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameHiddenPlayer : PlayerBehaviour
{
    [SerializeField] Transform player;
    MeshRenderer meshRenderer;
    [SerializeField] float stunDuration, visibilityDuration;
    bool canKill = true;
    private Coroutine stunCycle;

    protected override void Tick()
    {
    }


    public void Stun(){
        if(stunCycle != null)StopCoroutine(stunCycle);
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
    }

}
