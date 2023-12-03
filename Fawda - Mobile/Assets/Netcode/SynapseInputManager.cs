using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class SynapseInputManager : MonoBehaviour
{

    [SerializeField]
    TouchStickInput touchStickInput;

    [SerializeField]
    InputType inputMode;
    bool polling = false;

    UnityAction flash;

    //C
    public void BeginPolling(){
        flash = () => ClientConnection.singleton.FlashMessageToServer((OpCode)inputMode, PollForInput());
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



    public virtual byte[] PollForInput(){
        float[] stickVal = touchStickInput.PollInput();
        GamepadData gamepadData = new GamepadData(stickVal[0],stickVal[1]);
        return gamepadData.Encode();
    }
}
