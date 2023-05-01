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

    public void ChangeInputMode(int _newMode){
        inputMode = _newMode;
    }


    public void Update(){
        switch (inputMode)
        {
            case 0:
                print("WE POLLING");
                ClientConnection.singleton.FlashMessageToServer(OpCode.UDP_GAMEPAD_INPUT, PollForInput());
                break;
        }
    }

    public byte[] PollForInput(){
        Vector2 stickVal = touchStickInput.PollInput();
        byte[] xBytes = BitConverter.GetBytes(stickVal.x);
        byte[] yBytes = BitConverter.GetBytes(stickVal.y);
        byte[] press = BitConverter.GetBytes(gamepadButtonInput.PollInput());
        byte[] gamepadBytes = new byte[xBytes.Length + yBytes.Length + press.Length];
        Buffer.BlockCopy(xBytes,0,gamepadBytes,0,xBytes.Length);
        Buffer.BlockCopy(yBytes,0,gamepadBytes,xBytes.Length,yBytes.Length);
        Buffer.BlockCopy(press,0,gamepadBytes,xBytes.Length+yBytes.Length,press.Length);
        return gamepadBytes;
    }
}
