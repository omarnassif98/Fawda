using System;
using UnityEngine;
public class GameScreenManager : ScreenManager
{
    [SerializeField] SynapseInputManager menuStickManager;
    
    // Start is called before the first frame update
    void Start()
    {
       ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
    }

    public void SummonMenuControls(){
        ModalManager.singleton.SummonModal(0);
        ModalManager.singleton.AddDismissalListener(menuStickManager.EndPolling);
        ModalManager.singleton.AddDismissalListener(() => ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,false));
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,true);
        menuStickManager.BeginPolling();
    }

}
