using System;
using System.Collections.Generic;
using UnityEngine;
public class GameScreenManager : ScreenManager
{
    [SerializeField] SynapseInputManager menuStickManager;
    Dictionary<GameCodes,int> gameSetupScreens = new Dictionary<GameCodes, int>();
    
    void Awake(){
        gameSetupScreens[GameCodes.HAUNT] = 2;
    }

    // Start is called before the first frame update
    void Start()
    {
       ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
       ClientConnection.singleton.RegisterRPC("GAMESETUP",(byte[] _data) => this.SwitchSubscreens(gameSetupScreens[(GameCodes)_data[0]]));
    }


    public void SummonMenuControls(){
        ModalManager.singleton.SummonModal(0);
        ModalManager.singleton.AddDismissalListener(menuStickManager.EndPolling);
        ModalManager.singleton.AddDismissalListener(() => ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,false));
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_OCCUPY,true);
        menuStickManager.BeginPolling();
    }

}
