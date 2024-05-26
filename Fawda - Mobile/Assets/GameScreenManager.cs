using System;
using System.Collections.Generic;
using UnityEngine;
public class GameScreenManager : ScreenManager
{
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


    public void ToggleMenuControls(bool _newVal){
        Orchestrator.inputHandler.SetPollActivity(_newVal);
        transform.parent.parent.GetComponent<Animator>().SetBool("gamepad",_newVal);
    }

}
