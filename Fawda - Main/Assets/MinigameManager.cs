using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField]
    Transform[] playerSpawnPoints;
    [SerializeField]
    GameObject gamepadPlayerPrefab;
    // Start is called before the first frame update
    public void SetupGame(){
        short[] playerIdxs = ConnectionManager.singleton.GetPlayerIdxs();
        for(int i = 0; i < playerIdxs.Length; i++){
            GameObject player = GameObject.Instantiate(gamepadPlayerPrefab,playerSpawnPoints[i].position, Quaternion.identity,transform);
            player.GetComponent<GamepadCharacterController>().Initialize(playerIdxs[i]);
        }
    }
}
