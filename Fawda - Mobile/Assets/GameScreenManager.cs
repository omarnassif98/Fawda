using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenManager : ScreenManager
{
    // Start is called before the first frame update
    void Start()
    {
       ClientConnection.singleton.RegisterServerEventListener("connect",() => this.SwitchSubscreens(1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
