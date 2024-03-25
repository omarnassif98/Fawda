using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameHiddenPlayer : PlayerBehaviour
{
    [SerializeField] Transform player;
    MeshRenderer meshRenderer;

    protected override void Tick()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            meshRenderer.enabled = hit.collider.transform == player && Vector3.Angle(player.forward, transform.position - player.position) < 20;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

}
