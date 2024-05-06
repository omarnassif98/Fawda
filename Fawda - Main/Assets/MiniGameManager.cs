using System;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private DeployableMinigame currentMinigame;

    public void IntroduceMinigame(string _mode){
        Type minigameLogic = Type.GetType(string.Format("{0}GameManager",_mode));
        if(minigameLogic == null){
            DebugLogger.singleton.Log(string.Format("{0}GameManager is not a minigame, dipshit", _mode));
            return;
        }
        ConnectionManager.singleton.SendMessageToClients(OpCode.GAMESETUP, (int)Enum.Parse(typeof(GameCodes), _mode.ToUpper()));
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
