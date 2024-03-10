using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] Transform lookAtBall;
    const float speed =  4.5f;
    void Update(){
        Move();
        Tick();
    }
    // Update is called once per frame
    protected void Move()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * speed;
        Vector2 rotInput = new Vector2(Input.GetAxisRaw("Debug Horizontal"), Input.GetAxisRaw("Debug Vertical"));
        if(rotInput == Vector2.zero) return;
        lookAtBall.position = transform.position + new Vector3(rotInput.x, 0, rotInput.y);
        transform.LookAt(lookAtBall);
    }

    protected abstract void Tick();
}