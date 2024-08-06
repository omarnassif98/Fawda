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
        UIManager.bannerUIBehaviour.FlashMessage("One of Y'all is gonna be picked", 0.25f);
        UIManager.bannerUIBehaviour.AddBannerMessage("Who's first?");
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
        UIManager.bannerUIBehaviour.ClearBannerMessage();
        string name = LobbyManager.players[promptIdx].name;
        UIManager.bannerUIBehaviour.AddBannerMessage(name, 1);
        UIManager.bannerUIBehaviour.AddBannerMessage("Check", 0.25f);
        UIManager.bannerUIBehaviour.AddBannerMessage("Your", 0.25f);
        UIManager.bannerUIBehaviour.AddBannerMessage("Phone", 1);

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
        UIManager.RosterManager.SetTickerPosition(promptIdx);

        DebugLogger.SourcedPrint("PromptedGameSetup", "Ghost is client #" + promptIdx, "00FF00");
        //BOOKMARK: INFORMING PLAYERS
        for (int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++) UpdateGlyphs(i, false);

        UIManager.bannerUIBehaviour.ClearBannerMessage();
        UIManager.bannerUIBehaviour.AddBannerMessage("Ready?", 0.5f);
        for (int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++) ConnectionManager.singleton.SendMessageToClients(OpCode.READYUP, new SimpleBooleanMessage(i == promptIdx).Encode(), i);
        LobbyManager.gameManager.ConfigureGame(promptIdx);
        ConnectionManager.singleton.VacateRPC(OpCode.PROMPT_RESPONSE);
        TriggerMapLoad();
        BeginReady();
    }

}
