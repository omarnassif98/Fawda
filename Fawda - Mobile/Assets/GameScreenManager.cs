using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameScreenManager : ScreenManager
{
    [SerializeField] Button takeMenuControlButton;
    [SerializeField] SynapseInputManager menuStickManager;
    [SerializeField] TMP_Text debug;
    
    // Start is called before the first frame update
    void Start()
    {
       ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
       ClientConnection.singleton.RegisterServerEventListener("connect",() => ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPATION_STATUS));
       ClientConnection.singleton.RegisterRPC(OpCode.MENU_OCCUPATION_STATUS,UpdateMenuControl); 
         
    }

    public void UpdateMenuControl(byte[] _data){
        bool occupied = BitConverter.ToBoolean(_data,0);
        int occupiedIdx = BitConverter.ToInt32(_data,1);
        takeMenuControlButton.interactable = !occupied;
        string message = (!occupied)?"Take Control":"Wait Your Turn";
        debug.text = string.Format("Occ {0}", occupiedIdx);
        takeMenuControlButton.transform.Find("text").GetComponent<TMP_Text>().text = message;
        print(string.Format("Menu ctrl msg: occupied = {0} occupiedIdx = {1} Idx = {2}", occupied, occupiedIdx, ClientConnection.singleton.GetPlayerIdx()));
        if (!occupied || !(occupied && (occupiedIdx == ClientConnection.singleton.GetPlayerIdx()))) return;
        ModalManager.singleton.SummonModal(0);
        ModalManager.singleton.AddDismissalListener(() => SendMenuOccupation(false));
        ModalManager.singleton.AddDismissalListener(menuStickManager.EndPolling);
        menuStickManager.BeginPolling();
    }

    public void SendMenuOccupation(bool _occupied){
        debug.text = _occupied?"RAH":"Normal";
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,BitConverter.GetBytes(_occupied));
    }
}
