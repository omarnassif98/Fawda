using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameDeployable : DeployableAsymetricMinigame
{
    private GameObject hunterPlayerPrefab, ghostPlayerPrefab;
    

    public HauntGameDeployable(){
        DebugLogger.SourcedPrint("HauntGameDeployable","Deployed", "00FFFF");
        hunterPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/HunterPlayer") as GameObject;
        ghostPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/GhostPlayer") as GameObject;
    }


    public override void SetupGame(Transform _mapWrapper, int _specialityPlayer)
    {
        DebugLogger.SourcedPrint("HauntDeployableInstance", "Setup");
        base.SetupGame(_mapWrapper, _specialityPlayer);
        ProfileData[] playerProfiles = LobbyManager.players;
        DebugLogger.SourcedPrint("HauntGameDeployable","Map Generating", ColorUtility.ToHtmlStringRGB(Color.cyan));
        HauntGameMapGenerator waveCollapse = new HauntGameMapGenerator(_mapWrapper);
        waveCollapse.GenerateFloormap();
        int currentHunterSpawnPointIdx = 0;
        for(int i = 0; i < playerProfiles.Length; i++){
            if(playerProfiles[i] == null) continue;
            DebugLogger.SourcedPrint("HauntGameDeployable","Spawning player " + i.ToString(), ColorUtility.ToHtmlStringRGB(Color.cyan));
            if(i == asymetricPlayerIdx){
                playerInstances[i] = GameObject.Instantiate(ghostPlayerPrefab, waveCollapse.ghostSpawnPoint.position + Vector3.up * (HauntGameMapGenerator.FLOOR_THICKNESS + 0.1f), waveCollapse.ghostSpawnPoint.rotation, _mapWrapper).GetComponent<HauntHiddenPlayerBehaviour>();
            } else{
                playerInstances[i] = GameObject.Instantiate(hunterPlayerPrefab, waveCollapse.hunterSpawnPoints[currentHunterSpawnPointIdx].position + Vector3.up * (HauntGameMapGenerator.FLOOR_THICKNESS + 0.1f), waveCollapse.hunterSpawnPoints[currentHunterSpawnPointIdx].rotation, _mapWrapper).GetComponent<HauntHunterPlayerBehaviour>();
                currentHunterSpawnPointIdx += 1;
            }
        }
    }

    protected override IEnumerator ShowTutorialIntro()
    {
        DebugLogger.SourcedPrint("HauntGameDeployable", "Tutorial start");
        yield return new WaitForSecondsRealtime(1.5f);
        gameInPlay = true;
        DebugLogger.SourcedPrint("HauntGameDeployable", "Tutorial end");
    }

    protected override IEnumerator WindDownGame()
    {
        gameInPlay = false;
        DebugLogger.SourcedPrint("Haunt", "END");
        yield return new WaitForSecondsRealtime(2.3f);
        DioramaControllerBehaviour.singleton.SetCameraMode(false);
    }
}
