using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager
{

    private readonly Dictionary<GameCodes, Tuple<Type,Type>> minigameLookup = new Dictionary<GameCodes,Tuple<Type,Type>>()
    {
        {GameCodes.HAUNT, new Tuple<Type, Type>(typeof(HauntGameDeployable),typeof(HauntGameSetupBehaviour))}
    };

    public GameSetupBehaviour activeMinigameSetup {get; private set;}
    public DeployableMinigame activeMinigame {get; private set;}
    public Transform mapWrapper { get; private set; }

    [HideInInspector]
    public UnityEvent GameStartEvent = new UnityEvent(), GameEndEvent = new UnityEvent(), GamePauseEvent = new UnityEvent();




    public GameManager(Transform _maptransform){
        DebugLogger.SourcedPrint("GameManager", "Awake");
        mapWrapper = _maptransform;
    }

    public void LoadMinigame(GameCodes _gamecode){
        if(activeMinigame != null) return;

        DioramaControllerBehaviour.singleton.SetCameraMode(true);
        Tuple<Type,Type> gameInfo = minigameLookup[_gamecode];
        activeMinigame = (DeployableMinigame)Activator.CreateInstance(gameInfo.Item1);
        activeMinigame.LoadMap();
        activeMinigameSetup = (GameSetupBehaviour)Activator.CreateInstance(gameInfo.Item2);
        ConnectionManager.singleton.SendMessageToClients(OpCode.GAMESETUP, (int)GameCodes.HAUNT);

        DebugLogger.SourcedPrint("GameManager",string.Format("Minigame Loaded - {0}",Enum.GetName(typeof(GameCodes),_gamecode)));
    }

    public void KillMinigame(){
        activeMinigame = null;
        DebugLogger.SourcedPrint("GameManager", "Minigame Killed");
    }

    public void ConfigureGame(int _playerIdx = -1)
    {
        DebugLogger.SourcedPrint("GameManager", "Reset map");
        if(activeMinigame is DeployableAsymetricMinigame) ((DeployableAsymetricMinigame)activeMinigame).RegisterAsymetricPlayer(_playerIdx);
        activeMinigame.SpawnPlayers();
    }

    public void StartGame() => activeMinigame.StartGame();

    public void EndMinigame() => activeMinigame.EndGame();


}
