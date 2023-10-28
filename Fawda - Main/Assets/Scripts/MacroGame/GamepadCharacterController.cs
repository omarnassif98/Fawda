using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadCharacterController : MonoBehaviour
{
   short playerIdx = -1;
   [SerializeField]
   float speed,maxSpeed;
   public void Initialize(short _idx){
    playerIdx= _idx;
   }

   void Update(){
    ControlCharacter(InputManager.singleton.PullJoypadState(playerIdx));
   }

   void ControlCharacter(JoypadState _joypad){
    speed = Mathf.Lerp(speed,_joypad.analog.magnitude * maxSpeed,1/1.5f);
    transform.position = transform.position + (new Vector3(_joypad.analog.x, 0, _joypad.analog.y) * speed * Time.deltaTime);

   }
}
