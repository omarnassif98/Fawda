using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameScreenManager : ScreenManager
{

    struct SubscreenAlphaElements{
        public TMP_Text[] textElements;
        public Image[] images;
        public SubscreenAlphaElements(Transform _transform){
            textElements = _transform.GetComponentsInChildren<TMP_Text>();
            images = _transform.GetComponentsInChildren<Image>();
            DebugLogger.SourcedPrint("GameScreenManager", _transform.name + " has " + (images.Length + textElements.Length) + " elements");
        }
    }
    private SubscreenAlphaElements[] subscreenAlphaElements;
    private Dictionary<GameCodes,int> gameSetupScreens = new Dictionary<GameCodes, int>();

    public GameScreenManager(Transform _transform) : base(_transform)
    {
        gameSetupScreens[GameCodes.HAUNT] = 2;
        ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
        ClientConnection.singleton.RegisterRPC(OpCode.GAMESETUP,(byte[] _data) => this.SwitchSubscreens(gameSetupScreens[(GameCodes)_data[0]]));
        subscreenAlphaElements = new SubscreenAlphaElements[_transform.childCount];
        for(int i = 0; i < subscreenAlphaElements.Length; i++) subscreenAlphaElements[i] = new SubscreenAlphaElements(_transform.GetChild(i));
    }





}
