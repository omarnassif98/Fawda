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
        int occIdx = BitConverter.ToInt32(_data,1);
        takeMenuControlButton.interactable = !occ;
        string message = (!occ)?"Take Control":"Wait Your Turn";
        takeMenuControlButton.transform.Find("text").GetComponent<TMP_Text>().text = message;
        print(string.Format("Menu ctrl msg: occ = {0} occIdx = {1} Idx = {2}", occ, occIdx, ClientConnection.singleton.GetPlayerIdx()));
        if (!occ || !(occ && (occIdx == ClientConnection.singleton.GetPlayerIdx()))) return;
        SynapseInputManager.singleton.BeginPolling(InputMode.Menu);
        ModalManager.singleton.SummonModal(0);
        ModalManager.singleton.AddDismissalListener(() => SendMenuOccupation(false));
        ModalManager.singleton.AddDismissalListener(SynapseInputManager.singleton.EndPolling);
    }

    public void SendMenuOccupation(bool _occ){
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,BitConverter.GetBytes(_occ));
    }
}
