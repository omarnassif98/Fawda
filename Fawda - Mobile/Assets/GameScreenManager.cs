using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class GameScreenManager : ScreenManager
{

    private Dictionary<GameCodes,int> gameSetupScreens = new Dictionary<GameCodes, int>();

    public GameScreenManager(Transform _transform) : base(_transform)
    {
        gameSetupScreens[GameCodes.HAUNT] = 2;
        ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
        ClientConnection.singleton.RegisterRPC(OpCode.GAMESETUP,(byte[] _data) => this.SwitchSubscreens(gameSetupScreens[(GameCodes)_data[0]]));
        _transform.Find("LobbySubscreen/LobbyMenuControlsButton").GetComponent<Button>().onClick.AddListener(() => {
            SwitchSubscreens(2);
            Orchestrator.singleton.menuUIHandler.SetTabVisibility(false);
            Orchestrator.inputHandler.SetPollActivity(true);
            _transform.Find("GamepadSubscreen/BackButton").GetComponent<Button>().onClick.AddListener(() => {
                SwitchSubscreens(1);
                Orchestrator.singleton.menuUIHandler.SetTabVisibility(true);
                Orchestrator.inputHandler.SetPollActivity(false);
                _transform.Find("GamepadSubscreen/BackButton").GetComponent<Button>().onClick.RemoveAllListeners();
            });
    });
    }
}
