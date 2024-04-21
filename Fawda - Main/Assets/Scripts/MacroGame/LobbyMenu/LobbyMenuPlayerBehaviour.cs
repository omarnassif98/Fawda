using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuPlayerBehaviour : PlayerBehaviour
{
    protected override void Tick()
    {
        return;
    }

    protected override void Awake(){
        hotseat = this;
        playerDefaultMaterial = Resources.Load("Global/Materials/PlayerMat") as Material;
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
