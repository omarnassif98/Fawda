using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    IEnumerator currentPromptAction;
    int promptIdx = 0;
    public HauntGameSetupBehaviour() : base(){
        DebugLogger.SourcedPrint("HauntGameSetup", "Now waiting for readies");
        currentPromptAction = PromptPlayerForGhost();
        LobbyManager.singleton.StartCoroutine(currentPromptAction);
        ConnectionManager.singleton.RegisterRPC(OpCode.PROMPT_RESPONSE, promptAnswer);
    }
    public override void ReadyUp()
    {

    }

    void pushPromptForward(){
        promptIdx = (promptIdx + 1) % LobbyManager.singleton.GetLobbySize();
        currentPromptAction = PromptPlayerForGhost();
        LobbyManager.singleton.StartCoroutine(currentPromptAction);
    }


    IEnumerator PromptPlayerForGhost(){
        DebugLogger.SourcedPrint("HauntGameSetup","Sending prompt to client " + promptIdx);
        //BOOKMARK: PROMPT
        ConnectionManager.singleton.SendMessageToClients(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(true).Encode(), promptIdx);
        yield return new WaitForSeconds(7.5f);
        pushPromptForward();
    }

    void promptAnswer(byte[] _data, int _idx){
        DebugLogger.SourcedPrint("HauntGameSetup","Client " + promptIdx + " response", "FF0000");
        LobbyManager.singleton.StopCoroutine(currentPromptAction);
        bool accepted = new SimpleBooleanMessage(_data).ready;
        if (!accepted){
            DebugLogger.SourcedPrint("HauntGameSetup","Client " + promptIdx + " said no... the dumbass", "FF0000");
            pushPromptForward();
            return;
        }
        DebugLogger.SourcedPrint("HauntGameSetup","Ghost is client #" + promptIdx, "00FF00");
        //BOOKMARK: INFORMING PLAYERS
        for(int i = 0; i < LobbyManager.singleton.GetLobbySize(); i++) ConnectionManager.singleton.SendMessageToClients(OpCode.READYUP, new SimpleBooleanMessage(i == promptIdx).Encode(), i);
        ConnectionManager.singleton.VacateRPC(OpCode.PROMPT_RESPONSE);
    }

    protected override void OnReadyStatusChange(int _idx, bool _newVal)
    {
        DebugLogger.SourcedPrint("HauntGameSetup", "readyup logic trip idx: " + _idx);
    }
}
