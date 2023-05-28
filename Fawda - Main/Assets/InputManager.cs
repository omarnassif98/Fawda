using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public struct JoypadState{
    public Vector2 analog;
    public bool action;
    public JoypadState(Vector2 _analog, bool _action){
        analog = _analog;
        action = _action;
    }
}

public class InputManager : MonoBehaviour
{
    public static InputManager singleton;
    [SerializeField]
    private JoypadState[] joypadStates = new JoypadState[5];

    private UnityAction<Vector2,bool>[] gamepadPollEvents = new UnityAction<Vector2,bool>[5];
    float pollStartTime;
    int[] messageHist = new int[5];
    
    void Awake(){
        if(singleton != null){
            Destroy(this);
        }else{
            singleton = this;
        }
    }
    
    public void RegisterPollEvent(UnityAction<Vector2,bool> _gamepadPollEvent, int _idx){
        print("we register poll events??");
        gamepadPollEvents[_idx] = _gamepadPollEvent;
    }

    public void BeginTiming(){
        pollStartTime = Time.time;
    }
    public void Start(){
        ConnectionManager.singleton.RegisterRPC(Enum.GetName(typeof(OpCode), OpCode.UDP_GAMEPAD_INPUT), ReceivePoll);
    }
    public void ReceivePoll(byte[] _data, int _idx){
        float dirInput = BitConverter.ToSingle(_data,0);
        float distInput = BitConverter.ToSingle(_data,4);
        bool buttonInput = BitConverter.ToBoolean(_data,8);
        Vector2 stick = new Vector2(Mathf.Cos(dirInput), Mathf.Sin(dirInput)) * distInput;
        messageHist[_idx]++;
        joypadStates[_idx] = new JoypadState(stick, buttonInput);
    }

    public JoypadState PullJoypadState(short _idx){
        return joypadStates[_idx];
    }

    public float GetIndexHertz(int _idx){
        return messageHist[_idx]/(Time.time - pollStartTime);
    }
}
