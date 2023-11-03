using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameScreenManager : ScreenManager
{
    [SerializeField] Button takeMenuControlButton;
    // Start is called before the first frame update
    void Start()
    {
       ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
       ClientConnection.singleton.RegisterServerEventListener("connect",() => ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPATION_STATUS));
       ClientConnection.singleton.RegisterRPC(OpCode.MENU_OCCUPATION_STATUS,UpdateMenuControl);
       
    }

    public void UpdateMenuControl(byte[] _data){
        bool occ = BitConverter.ToBoolean(_data,0);
        takeMenuControlButton.interactable =!occ;
        string message = (!occ)?"Take Control":"Wait Your Turn";
        takeMenuControlButton.transform.Find("text").GetComponent<TMP_Text>().text = message;
    }

    public void RequestMenuOccupation(){
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY);
    }
}
