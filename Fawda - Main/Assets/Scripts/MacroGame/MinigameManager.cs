using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager singleton;
    private DeployableMinigame currentMiniGame;

    public void Awake(){
        if(singleton == null){
            Destroy(this);
        }else{
            singleton = this;
        }
    }

    public void DeployMinigame(){
        
    }
    
}
