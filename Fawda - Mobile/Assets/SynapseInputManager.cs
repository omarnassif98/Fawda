using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SynapseInputManager : MonoBehaviour
{
    public static SynapseInputManager singleton;
    int inputMode = -1;

    [SerializeField]
    TouchStickInput touchStickInput;

    [SerializeField]
    GamepadButtonInput gamepadButtonInput;

    public void Awake(){
        if(singleton !=null){
            Destroy(this);
        }else{
            singleton = this;
        }
    }

    //C
    public void ChangeInputMode(int _newMode){
        inputMode = _newMode;
        StartCoroutine(FlashInput());
    }


    IEnumerator FlashInput(){
        while(inputMode != -1){
            switch (inputMode)
            {
                case 0:
                    ClientConnection.singleton.FlashMessageToServer(OpCode.UDP_GAMEPAD_INPUT, PollForInput());
                    break;
            }
            yield return new WaitForSeconds(1.0f/60);
        }
        print("Input stream break");
        yield return null;
    }

    public byte[] PollForInput(){
        float[] stickVal = touchStickInput.PollInput();
        byte[] dirBytes = BitConverter.GetBytes(stickVal[0]);
        byte[] distBytes = BitConverter.GetBytes(stickVal[1]);
        byte[] press = BitConverter.GetBytes(gamepadButtonInput.PollInput());
        byte[] gamepadBytes = new byte[dirBytes.Length + distBytes.Length + press.Length];
        Buffer.BlockCopy(dirBytes,0,gamepadBytes,0,dirBytes.Length);
        Buffer.BlockCopy(distBytes,0,gamepadBytes,dirBytes.Length,distBytes.Length);
        Buffer.BlockCopy(press,0,gamepadBytes,dirBytes.Length+distBytes.Length,press.Length);
        //print(gamepadBytes.Length);
        return gamepadBytes;
    }
}
