using UnityEngine;
using System;
using UnityEngine.Events;

public struct JoypadState{
    public Vector2 analog;
    public bool action;
    public JoypadState(GamepadData _gamepadData){
        analog = new Vector2(_gamepadData.xInput, _gamepadData.yInput);
        action = BitConverter.ToBoolean(_gamepadData.additionalInfo);
    }
}
public class InputManager
{
    public static JoypadState[] joypadStates {get; private set;}
    public static UnityEvent[] heartbeatEvents {get; private set;}
    public InputManager(){
        joypadStates = new JoypadState[5];
        heartbeatEvents = new UnityEvent[5];
        for(int i = 0; i < 5; i++){
            heartbeatEvents[i] = new UnityEvent();
        }
        ConnectionManager.singleton.RegisterRPC(Enum.GetName(typeof(OpCode), OpCode.UDP_GAMEPAD_INPUT), ReceivePoll);
        DebugLogger.SourcedPrint("Input Manager", "Awake and ready");
    }

    public void ReceivePoll(byte[] _data, int _idx){
        GamepadData gamepadData = new GamepadData(_data);
        joypadStates[_idx] = new JoypadState(gamepadData);
        heartbeatEvents[_idx].Invoke();
    }

}
