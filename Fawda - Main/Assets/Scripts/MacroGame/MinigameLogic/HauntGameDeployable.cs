using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameDeployable : DeployableAsymetricMinigame
{
    private GameObject hunterPlayerPrefab, ghostPlayerPrefab;
    private HauntGameMapGenerator generator;

    public HauntGameDeployable(){
        DebugLogger.SourcedPrint("HauntGameDeployable","Deployed", "00FFFF");
        hunterPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/HunterPlayer") as GameObject;
        ghostPlayerPrefab = Resources.Load("MinigameAssets/Haunt/Prefabs/GhostPlayer") as GameObject;
        LocateMapTransform();
    }


    public override void RegisterAsymetricPlayer(int _specialityPlayer)
    {
        DebugLogger.SourcedPrint("HauntDeployableInstance", "Setup");
        base.RegisterAsymetricPlayer(_specialityPlayer);
        
    }

    public override void LoadMap()
    {
        base.LoadMap();
        DebugLogger.SourcedPrint("HauntGameDeployable", "Map Generating", ColorUtility.ToHtmlStringRGB(Color.cyan));
        generator = new HauntGameMapGenerator(transform);
        generator.GenerateFloormap();
    }

    public override void SpawnPlayers()
    {
        int currentHunterSpawnPointIdx = 0;
        ProfileData[] playerProfiles = LobbyManager.players;
        for (int i = 0; i < playerProfiles.Length; i++)
        {
            if (playerProfiles[i] == null) continue;
            DebugLogger.SourcedPrint("HauntGameDeployable", "Spawning player " + i.ToString(), ColorUtility.ToHtmlStringRGB(Color.cyan));
            if (i == asymetricPlayerIdx)
            {
                playerInstances[i] = GameObject.Instantiate(ghostPlayerPrefab, generator.ghostSpawnPoint.position + Vector3.up * (HauntGameMapGenerator.FLOOR_THICKNESS + 0.1f), generator.ghostSpawnPoint.rotation, transform).GetComponent<HauntHiddenPlayerBehaviour>();
            }
            else
            {
                playerInstances[i] = GameObject.Instantiate(hunterPlayerPrefab, generator.hunterSpawnPoints[currentHunterSpawnPointIdx].position + Vector3.up * (HauntGameMapGenerator.FLOOR_THICKNESS + 0.1f), generator.hunterSpawnPoints[currentHunterSpawnPointIdx].rotation, transform).GetComponent<HauntHunterPlayerBehaviour>();
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
