using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{

    void Update(){
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 4.5f;    
    }

    protected abstract void Tick();
}