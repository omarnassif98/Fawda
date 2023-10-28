using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    int idx = 0;
    [SerializeField] ScreenManager[] screens;
    public static UIManager singleton;
      private void Awake(){
        if(singleton != null && singleton != this){
            Destroy(this);
        }else{
            singleton = this;
        }
    }
    
    public void Start(){
        ClientConnection.singleton.RegisterServerEventListener("connect",kickoff);
        ClientConnection.singleton.RegisterRPC(Enum.GetName(typeof(OpCode), OpCode.UDP_TOGGLE), ToggleControls);
    }

    private void ToggleControls(byte[] _data){
        SwitchScreens(BitConverter.ToBoolean(_data,0)?3:2);
        print("Controls: " + ((bool)BitConverter.ToBoolean(_data,0)).ToString());
    }

    private void kickoff(){
        SwitchScreens(2);
    }

    public void SwitchScreens(int _newIdx){
        print("SWITCH NEEDED");
        screens[idx].DismissScreen();
        screens[_newIdx].IntroduceScreen();
        idx = _newIdx;
    }

    public void ManipulateMenu(int dir){
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_CONTROL, dir);
    }
}
