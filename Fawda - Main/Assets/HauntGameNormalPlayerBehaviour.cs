using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameNormalPlayerBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 4.5f;    
    }
}
