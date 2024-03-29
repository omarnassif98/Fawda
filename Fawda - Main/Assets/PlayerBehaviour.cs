using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour hotseat; //Debug var

    const float speed =  4.5f;
    void Update(){
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        if(hotseat != this) return;
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * speed;
    }

    protected abstract void Tick();
}