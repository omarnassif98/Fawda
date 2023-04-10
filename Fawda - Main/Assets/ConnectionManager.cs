using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager singleton;
    SynapseServer server;
    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        server = new SynapseServer();
        server.KickoffServer();
    }

    public void PrintWrap(string _msg){
        Debug.Log(string.Format("<color=#FFBF00>Synapse Client: </color>{0}",_msg));
    }

    public void OnDestroy(){
        server.Kill();
        PrintWrap("Quit");
    }
}
