using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public enum InputMode{
    Menu = 0,
    Joypad = 1
} 
public class SynapseInputManager : MonoBehaviour
{
    public static SynapseInputManager singleton;

    [SerializeField]
    TouchStickInput[] touchStickInput;

    [SerializeField]
    GamepadButtonInput gamepadButtonInput;

    InputMode inputMode;

    bool polling = false;

    UnityAction flash;

    public void Awake(){
        if(singleton !=null){
            Destroy(this);
        }else{
            singleton = this;
        }
    }

    //C
    public void BeginPolling(InputMode _inputMode){
        inputMode = _inputMode;

        switch (inputMode)
            {
                case InputMode.Menu:
                    flash = () => ClientConnection.singleton.FlashMessageToServer(OpCode.MENU_CONTROL, PollForMenuInput());
                    break;
                case InputMode.Joypad:
                    flash = () => ClientConnection.singleton.FlashMessageToServer(OpCode.UDP_GAMEPAD_INPUT, PollForInput());
                    break;
            }
        polling = true;
        StartCoroutine(FlashInput());
    }

    public void EndPolling(){
        polling = false;
    }

    
    IEnumerator FlashInput(){
        while(polling){
            flash();
            yield return new WaitForSeconds(1.0f/60);
        }
        print("Input stream break");
        yield return null;
    }



    public byte[] PollForMenuInput(){
        float[] stickVal = touchStickInput[(int)inputMode].PollInput();
        byte[] dirBytes = BitConverter.GetBytes(stickVal[0]);
        byte[] distBytes = BitConverter.GetBytes(stickVal[1]);
        byte[] gamepadBytes = new byte[dirBytes.Length + distBytes.Length];
        Buffer.BlockCopy(dirBytes,0,gamepadBytes,0,dirBytes.Length);
        Buffer.BlockCopy(distBytes,0,gamepadBytes,dirBytes.Length,distBytes.Length);
        //print(gamepadBytes.Length);
        return gamepadBytes;
    }

    public byte[] PollForInput(){
        float[] stickVal = touchStickInput[(int)inputMode].PollInput();
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
