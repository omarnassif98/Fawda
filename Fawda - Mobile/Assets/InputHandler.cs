using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler
{
    public Vector2 stickVal;
    public bool buttonVal;
    public bool polling { get; private set;}

    public InputHandler(){
        polling = false;
        stickVal = Vector2.zero;
        buttonVal = false;
    }

    public void SetPollActivity(bool _newVal){
        polling = _newVal;
    }

    public void PollInput(){
        if(!polling) return; //Even idle inputs are important
        GamepadData gamepadData = new GamepadData(stickVal.x, stickVal.y, buttonVal);
        ClientConnection.singleton.FlashMessageToServer(OpCode.UDP_GAMEPAD_INPUT, gamepadData.Encode());
    }
}
