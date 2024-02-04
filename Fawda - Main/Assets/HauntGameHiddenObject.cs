using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameHiddenObject : MonoBehaviour
{
    [SerializeField] Transform player;
    MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, player.position - transform.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            mr.enabled = hit.collider.transform == player && Vector3.Angle(player.forward, transform.position - player.position) < 20;
        }
    }
}
