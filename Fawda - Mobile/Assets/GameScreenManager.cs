using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class GameScreenManager : ScreenManager
{

    struct GameSetupLogic{
        public int screenIdx;
        public UnityAction kickoff;
        public GameSetupLogic(int _screenIdx, UnityAction _kickoff) {
            this.screenIdx = _screenIdx;
            this.kickoff = _kickoff;
        }
    }
    private Dictionary<GameCodes,GameSetupLogic> gameSetupScreens = new Dictionary<GameCodes, GameSetupLogic>();

    public GameScreenManager(Transform _transform) : base(_transform)
    {
        gameSetupScreens[GameCodes.HAUNT] = new GameSetupLogic(3, () => {
            ClientConnection.singleton.RegisterRPC(OpCode.PROMPT_RESPONSE, (_payload) => {
                    Transform mod = Orchestrator.singleton.menuUIHandler.SummonModal("GhostPrompt");
                    Orchestrator.singleton.menuUIHandler.AttachEphimeralToDismissEvent(() => {ClientConnection.singleton.SendMessageToServer(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(false).Encode());});
                    mod.Find("ghostconfirm").GetComponent<Button>().onClick.AddListener(() => {ClientConnection.singleton.SendMessageToServer(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(true).Encode()); Orchestrator.singleton.menuUIHandler.ClearDismissListeners(); Orchestrator.singleton.menuUIHandler.DismissModal();});
                    mod.Find("ghostdeny").GetComponent<Button>().onClick.AddListener(() => Orchestrator.singleton.menuUIHandler.DismissModal());
                    Orchestrator.singleton.StartCoroutine(Helper.LerpStep((_val) => {if(mod) mod.Find("timeleft").GetComponent<Image>().fillAmount = 1 - _val;}, () => {if(mod)Orchestrator.singleton.menuUIHandler.DismissModal();}, 5));
                });

            UnityAction<byte[]> setup = (_payload) => {
                _transform.Find("HauntSetupSubscreen/loadingtext").gameObject.SetActive(false);
                _transform.Find("HauntSetupSubscreen/readyupbutton").gameObject.SetActive(true);
                _transform.Find("HauntSetupSubscreen/readyupbutton").GetComponent<Button>().onClick.RemoveAllListeners();
                _transform.Find("HauntSetupSubscreen/readyupbutton").GetComponent<Button>().onClick.AddListener(() => ClientConnection.singleton.SendMessageToServer(OpCode.READYUP, new SimpleBooleanMessage(true).Encode()));
                if(BitConverter.ToBoolean(_payload,0) == true) _transform.Find("HauntSetupSubscreen/ghostprompt").gameObject.SetActive(true);
                else _transform.Find("HauntSetupSubscreen/hunterprompt").gameObject.SetActive(true);
                ClientConnection.singleton.ClearRPC(OpCode.READYUP);
            };
            ClientConnection.singleton.RegisterRPC(OpCode.READYUP, setup);
        });
        ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
        ClientConnection.singleton.RegisterRPC(OpCode.GAMESETUP,(byte[] _data) => {this.SwitchSubscreens(gameSetupScreens[(GameCodes)_data[0]].screenIdx); gameSetupScreens[(GameCodes)_data[0]].kickoff();});
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
