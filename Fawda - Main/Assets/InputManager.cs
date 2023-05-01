using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class InputManager : MonoBehaviour
{

    private UnityAction<Vector2,bool>[] gamepadPollEvents = new UnityAction<Vector2,bool>[5]; 
    public void RegisterPollEvent(UnityAction<Vector2,bool> _gamepadPollEvent, int _idx){
        gamepadPollEvents[_idx] = _gamepadPollEvent;
    }

    public void Start(){
        ConnectionManager.singleton.RegisterRPC(Enum.GetName(typeof(OpCode), OpCode.UDP_GAMEPAD_INPUT), ReceivePoll);
    }
    public void ReceivePoll(byte[] _data, int _idx){
        float xInput, yInput;
        byte[] xBytes = new byte[4], yBytes = new byte[4];
        ConnectionManager.singleton.PrintWrap("POLLING IN UDP TIME!!!");
        //bool buttonInput = 
        //Buffer.BlockCopy(_data,0,xBytes,0,4);
        //Buffer.BlockCopy(_data,4,yBytes,0,4);
        
    }
}
