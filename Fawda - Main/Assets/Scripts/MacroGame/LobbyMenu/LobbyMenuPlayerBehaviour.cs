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
        DioramaControllerBehaviour.singleton.TrackTransform(transform);
    }

    private void OnDestroy()
    {
        UntrackCamera();
    }

    private void UntrackCamera() => DioramaControllerBehaviour.singleton.StopTrackTransform(transform);

    // Start is called before the first frame update
    void Start()
    {

    }

}
