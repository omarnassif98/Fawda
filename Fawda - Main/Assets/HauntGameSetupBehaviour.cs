using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : GameSetupBehaviour
{
    List<int> readyOrder;
    int readyOrderIdx = 0;
    IEnumerator currentPromptAction;
    public HauntGameSetupBehaviour() : base(){
        DebugLogger.SourcedPrint("GameSetup", "Now waiting for readies");
        readyOrder = new List<int>();
    }
    public override void ReadyUp()
    {
        ConnectionManager.singleton.RegisterRPC(OpCode.PROMPT_RESPONSE, promptAnswer);
        currentPromptAction = PromptPlayerForGhost();
        LobbyManager.singleton.StartCoroutine(currentPromptAction);
    }

    void pushPromptForward(){
        readyOrderIdx = (readyOrderIdx + 1) % readyOrder.Count;
        currentPromptAction = PromptPlayerForGhost();
        LobbyManager.singleton.StartCoroutine(currentPromptAction);
    }


    IEnumerator PromptPlayerForGhost(){
        DebugLogger.SourcedPrint("HauntGameSetup","Sending prompt to client " + readyOrder[readyOrderIdx]);
        ConnectionManager.singleton.SendMessageToClients(OpCode.PROMPT_RESPONSE, new SimpleBooleanMessage(true).Encode(), readyOrder[readyOrderIdx]);
        yield return new WaitForSeconds(2.5f);
        pushPromptForward();
    }

    void promptAnswer(byte[] _data, int _idx){
        LobbyManager.singleton.StopCoroutine(currentPromptAction);
        bool accepted = new SimpleBooleanMessage(_data).ready;
        if (!accepted){
            DebugLogger.SourcedPrint("HauntGameSetup","Client " + readyOrder[readyOrderIdx] + " said no... the dumbass", "FF0000");
            pushPromptForward();
            return;
        }
        DebugLogger.SourcedPrint("HauntGameSetup","Ghost is client #" + readyOrder[readyOrderIdx], "00FF00");
        foreach(int i in readyOrder) ConnectionManager.singleton.SendMessageToClients(OpCode.CONTROL_SCHEME, new SimpleBooleanMessage(i == readyOrder[readyOrderIdx]).Encode(), i);
        ConnectionManager.singleton.VacateRPC(OpCode.PROMPT_RESPONSE);
    }

    protected override void OnReadyStatusChange(int _idx, bool _newVal)
    {
        DebugLogger.SourcedPrint("GameSetup", "logic trip");
        if(_newVal && !readyOrder.Contains(_idx)) readyOrder.Add(_idx);
        else if (!_newVal && readyOrder.Contains(_idx)) readyOrder.Remove(_idx);
        else DebugLogger.SourcedPrint("GameSetup", "Something seriously wrong ");
    }
}
