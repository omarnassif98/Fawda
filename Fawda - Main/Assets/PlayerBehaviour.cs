using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour hotseat; //Debug var
    protected bool isMobile = true;
    const float speed =  4.5f;
    protected Material playerDefaultMaterial;

    void Update(){
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        if(hotseat != this || !isMobile) return;
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * speed;
    }

    protected abstract void Tick();
}