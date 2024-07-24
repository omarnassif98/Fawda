using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PromptedGameSetupBehaviour : GameSetupBehaviour
{
    IEnumerator currentPromptAction;
    protected int promptIdx = -1;

    public PromptedGameSetupBehaviour() : base()
    {
        DebugLogger.SourcedPrint("PromptedGameSetup (grandfather logic)", "Now waiting for readies");
        UIManager.RosterManager.rosterEvent.AddListener((val) =>
        {
            promptIdx = val;
            ConnectionManager.singleton.RegisterRPC(OpCode.PROMPT_RESPONSE, PromptAnswer);
            currentPromptAction = PromptPlayer();
            LobbyManager.singleton.StartCoroutine(currentPromptAction);
        });
        UIManager.RosterManager.StartRoulette();
    }
   
    void PushPromptForward()
    {
        promptIdx = (promptIdx + 1) % LobbyManager.singleton.GetLobbySize();
        UIManager.RosterManager.SetTickerPosition(promptIdx);
        currentPromptAction = PromptPlayer();
        LobbyManager.singleton.StartCoroutine(currentPromptAction);

    }


    IEnumerator PromptPlayer()
    {
        DebugLogger.SourcedPrint("PromptedGameSetup", "Sending prompt to client " + promptIdx);
        //BOOKMARK: PROMPT
        ConnectionManager.singleton.SendMessageToClients(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(true).Encode(), promptIdx);
        yield return new WaitForSeconds(7.5f);
        PushPromptForward();
    }

    void PromptAnswer(byte[] _data, int _idx)
    {
        DebugLogger.SourcedPrint("PromptedGameSetup", "Client " + promptIdx + " response", "FF0000");
        LobbyManager.singleton.StopCoroutine(currentPromptAction);
        bool accepted = new SimpleBooleanMessage(_data).ready;
        if (!accepted)
        {
            DebugLogger.SourcedPrint("PromptedGameSetup", "Client " + promptIdx + " said no... the dumbass", "FF0000");
            PushPromptForward();
            return;
        }
        promptIdx = _idx; //I mean just in case a race condition happens
        DebugLogger.SourcedPrint("PromptedGameSetup", "Ghost is client #" + promptIdx, "00FF00");
        //BOOKMARK: INFORMING PLAYERS
        for (int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++) ConnectionManager.singleton.SendMessageToClients(OpCode.READYUP, new SimpleBooleanMessage(i == promptIdx).Encode(), i);
        LobbyManager.gameManager.ConfigureGame(promptIdx);
        ConnectionManager.singleton.RegisterRPC(OpCode.READYUP, ChangeReadyStatus);
        ConnectionManager.singleton.VacateRPC(OpCode.PROMPT_RESPONSE);
    }

}
