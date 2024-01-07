using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HauntSetupScreen : MonoBehaviour
{
    bool playerOptOut = false;
    bool ready = false;
    void Start(){
        UpdateGraphics();
    }
    public void ToggleIndicator(){
        playerOptOut = !playerOptOut;
        UpdateGraphics();
    }

    private void UpdateGraphics(){
        transform.Find("Default Option/Label").GetComponent<TMP_Text>().text = playerOptOut ? "Don't pick me as ghost":"Please pick me as ghost";
    }

    public void ReadyUp(){
        ArrayList config = new ArrayList();
        config.Add(playerOptOut);
        ready = !ready;
        PlayerGameConfigData gameConfigData = new PlayerGameConfigData(ready, config);
        ClientConnection.singleton.SendMessageToServer(OpCode.READYUP, gameConfigData.Encode());
    }

}
