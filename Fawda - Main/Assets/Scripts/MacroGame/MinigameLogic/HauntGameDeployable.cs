using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameDeployable : DeployableMinigame
{
    public HauntHiddenPlayerBehaviour hauntHiddenPlayerInstance{get; private set;}
    public HauntHunterPlayerBehaviour[] hauntHunterPlayerInstances{get; private set;}
    int ghostIdx = -1;

    public HauntGameDeployable(){
        DebugLogger.singleton.Log("Booyah");
    }

    public override void SetupGame(Transform _mapWrapper, int _specialityPlayer = -1)
    {
        DebugLogger.singleton.Log("HAUNT GAME CONFIGURED OH YEEEEEEEEAAH");
        ghostIdx = _specialityPlayer;
        ProfileData[] playerProfiles = LobbyManager.players;
        HauntGameMapGenerator waveCollapse = new HauntGameMapGenerator(_mapWrapper);
        waveCollapse.GenerateFloormap();


    }

    void PopulateInstances(){
        hauntHiddenPlayerInstance = GameObject.FindObjectOfType<HauntHiddenPlayerBehaviour>().GetComponent<HauntHiddenPlayerBehaviour>();
        hauntHunterPlayerInstances = GameObject.FindObjectsOfType<HauntHunterPlayerBehaviour>();
    }
}
