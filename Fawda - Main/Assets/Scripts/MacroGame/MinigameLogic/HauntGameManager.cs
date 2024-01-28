using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameManager : DeployableMinigame
{
    int ghostIdx = -1;
    public HauntGameManager(){
        DebugLogger.singleton.Log("Booyah");

    }

    public override void SetupGame(int _specialityPlayer = -1)
    {
        DebugLogger.singleton.Log("HAUNT GAME CONFIGURED OH YEEEEEEEEAAH");
        ghostIdx = _specialityPlayer;
        ProfileData[] playerProfiles = LobbyManager.singleton.GetPlayerProfiles();

        //MAP
        

        for(int i = 0, p = 0; i < playerProfiles.Length && p < LobbyManager.singleton.GetLobbySize() - 1; i++){
            if(ghostIdx != i && playerProfiles[i] != null){
                p += 1;
                //CREATE HUMAN
            }
        }

        //CREATE GHOST
    }


}
