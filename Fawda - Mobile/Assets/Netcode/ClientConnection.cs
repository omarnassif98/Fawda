using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections.Generic;

public class ClientConnection : MonoBehaviour
{
   public static ClientConnection singleton;
   SynapseClient client;
   [SerializeField] TMP_InputField code;
   [SerializeField] TMP_Text status;
   [SerializeField] TMP_Text addr_text;
   public Dictionary<string,UnityEvent> serverEvents;

    void Start(){
        client = new SynapseClient();
        status.text = "Not Connected";
        Debug.Log("LET ME SEE THIS");
    }

    private void Awake(){
        if(singleton != null && singleton != this){
            Destroy(this);
        }else{
            singleton = this;
            serverEvents = new Dictionary<string, UnityEvent>();
            serverEvents["connect"] = new UnityEvent();
        }
    }

    private string ParseRoomCode(string code){
        string addr = "";
        try{
        switch (code[0])
        {
            case '1':
            addr += "10";
            break;
            case '2':
            addr += "172.16";
            break;
            case '3':
            addr += "192.168";
            break;
        }

        for(int i = code.Length - 1; i > 0; i--){
            addr += '.';
            if(code[i] == 'G'){
                addr += "0";
            }else{
                addr += int.Parse(code.Substring(i-1,2),System.Globalization.NumberStyles.HexNumber);
                i--;
            }
        }
        }catch (Exception ex){
            Debug.LogException(ex,this);
        }
        return addr;
    }

    public void Connect(){
        status.text = "Connecting";
        //string addr = ParseRoomCode(code.text);
        string addr = code.text.Trim();
        addr_text.text = addr;
        client.kickoff(addr);
    }

    public void PrintWrap(string _message){
        print(string.Format("<color=#FFBF00>Synapse Client: </color>{0}",_message));
    }

    public void SendMessageToServer(OpCode _opCode, byte _val){
        NetMessage msg = new NetMessage(_opCode, _val);
        client.QueueMessage(msg);
    }

    public void TriggerServerEvent(string _event){
        if(serverEvents.ContainsKey(_event)){
            serverEvents[_event].Invoke();
        }
    }

    public void OnDestroy(){
        client.Kill();
    }
}
