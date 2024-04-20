using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameDeployable : DeployableMinigame
{
    public HauntHiddenPlayerBehaviour ghostPlayerInstance{get; private set;}
    public HauntHunterPlayerBehaviour[] hunterPlayerInstances{get; private set;}
    private GameObject hunterPlayerPrefab, ghostPlayerPrefab;
    int ghostIdx = -1;

    public HauntGameDeployable(){
        DebugLogger.singleton.Log("Booyah");
        hunterPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/HunterPlayer") as GameObject;
        ghostPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/GhostPlayer") as GameObject;
    }

    public override void SetupGame(Transform _mapWrapper, int _specialityPlayer = -1)
    {
        DebugLogger.SourcedPrint("Haunt Game Deployable","Deploying", ColorUtility.ToHtmlStringRGB(Color.cyan));
        ghostIdx = _specialityPlayer;
        ProfileData[] playerProfiles = LobbyManager.players;
        hunterPlayerInstances = new HauntHunterPlayerBehaviour[LobbyManager.singleton.GetLobbySize() - 1];
        HauntGameMapGenerator waveCollapse = new HauntGameMapGenerator(_mapWrapper);
        DebugLogger.SourcedPrint("Haunt Game Deployable","Map Generating", ColorUtility.ToHtmlStringRGB(Color.cyan));
        waveCollapse.GenerateFloormap();
        DebugLogger.SourcedPrint("Haunt Game Deployable","Map Generated", ColorUtility.ToHtmlStringRGB(Color.cyan));
        DebugLogger.SourcedPrint("Haunt Game Deployable","Special " + _specialityPlayer.ToString(), ColorUtility.ToHtmlStringRGB(Color.cyan));
        int currentHunterSpawnPointIdx = 0;
        for(int i = 0; i < playerProfiles.Length; i++){
            if(playerProfiles[i] == null) continue;
            DebugLogger.SourcedPrint("Haunt Game Deployable","Spawning player " + i.ToString(), ColorUtility.ToHtmlStringRGB(Color.cyan));
            if(i == _specialityPlayer){
                ghostPlayerInstance = GameObject.Instantiate(ghostPlayerPrefab, waveCollapse.ghostSpawnPoint.position, waveCollapse.ghostSpawnPoint.rotation, _mapWrapper).GetComponent<HauntHiddenPlayerBehaviour>();
            } else{
                hunterPlayerInstances[currentHunterSpawnPointIdx] = GameObject.Instantiate(hunterPlayerPrefab, waveCollapse.hunterSpawnPoints[currentHunterSpawnPointIdx].position, waveCollapse.hunterSpawnPoints[currentHunterSpawnPointIdx].rotation, _mapWrapper).GetComponent<HauntHunterPlayerBehaviour>();
                currentHunterSpawnPointIdx += 1;
            }
            gameInPlay = true;
        }
    }

}
