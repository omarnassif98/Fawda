using System;
using System.Collections.Generic;
using UnityEngine;
public class GameScreenManager : ScreenManager
{
    Dictionary<GameCodes,int> gameSetupScreens = new Dictionary<GameCodes, int>();

    public GameScreenManager(Transform _transform) : base(_transform)
    {
        gameSetupScreens[GameCodes.HAUNT] = 2;
        ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
        ClientConnection.singleton.RegisterRPC(OpCode.GAMESETUP,(byte[] _data) => this.SwitchSubscreens(gameSetupScreens[(GameCodes)_data[0]]));
    }

}
