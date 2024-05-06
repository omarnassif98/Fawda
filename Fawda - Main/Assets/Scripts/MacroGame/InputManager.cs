using UnityEngine;
using System;

public struct JoypadState{
    public Vector2 analog;
    public bool action;
    public JoypadState(Vector2 _analog, bool _action){
        analog = _analog;
        action = _action;
    }
}

public class InputManager
{
    public static JoypadState[] joypadStates {get; private set;}

    public InputManager(){
        joypadStates = new JoypadState[5];
        ConnectionManager.singleton.RegisterRPC(Enum.GetName(typeof(OpCode), OpCode.UDP_GAMEPAD_INPUT), ReceivePoll);
        DebugLogger.SourcedPrint("Input Manager", "Awake and ready");
    }

    public void ReceivePoll(byte[] _data, int _idx){
        float normalizedXInput = BitConverter.ToSingle(_data,0);
        float normalizedYInput = BitConverter.ToSingle(_data,4);
        bool buttonInput = BitConverter.ToBoolean(_data,8);
        Vector2 stick = new Vector2(normalizedXInput, normalizedYInput);
        joypadStates[_idx] = new JoypadState(stick, buttonInput);
    }

}
