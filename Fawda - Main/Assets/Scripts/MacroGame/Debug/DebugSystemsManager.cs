using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugSystemsManager : MonoBehaviour
{

        [SerializeField]
    TMP_Text[] DEBUG_Client_Statuses;

    private string[] DEBUG_Client_Status_Texts = new string[5];

    [SerializeField]
    TMP_Text DEBUG_SERVER_UP;

    private string DEBUG_SERVER_UP_TEXT;

    [SerializeField]
    TMP_Text DEBUG_SERVER_LISTEN;

    private string DEBUG_SERVER_LISTEN_TEXT;
    [SerializeField]
    private GameObject debug_bar, logger;

    [SerializeField]
    private DebugGamepadVisualization[] visualizations;

    public void SetDebug_Client_Status(int _idx, bool _connected){
        string status = _connected?"<color=#55FF00>Connected</color>":"<color=#FF0090>Disconnected</color>";
        DEBUG_Client_Status_Texts[_idx] = string.Format("Client {0}: {1}", _idx + 1, status);
    }

    public void SetDebug_Server_Status(bool _status){
        string status = _status?"<color=#55FF00>Server Up</color>":"<color=#FF0090>Server Down</color>";
        DEBUG_SERVER_UP_TEXT = status;
    }

    public void SetDebug_Listen_Status(bool _status){
        string status = _status?"<color=#55FF00>Listening</color>":"<color=#FF0090>Not Listening</color>";
        DEBUG_SERVER_LISTEN_TEXT = status;
    }

    private void RefreshDebugInfo(){
        for(int i = 0; i < DEBUG_Client_Statuses.Length; i++){
            DEBUG_Client_Statuses[i].text = DEBUG_Client_Status_Texts[i];
        }
        DEBUG_SERVER_UP.text = DEBUG_SERVER_UP_TEXT;
        DEBUG_SERVER_LISTEN.text = DEBUG_SERVER_LISTEN_TEXT;
    }

    void Update()
    {
        RefreshDebugInfo();
        if(Input.GetKeyDown(KeyCode.Tilde)){
            debug_bar.SetActive(!debug_bar.activeInHierarchy);
            logger.SetActive(!logger.activeInHierarchy);
        }
    }
}
