using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSetupBehaviour : MonoBehaviour
{
    [SerializeField] DeployableMinigame deployableMinigame;

    public abstract void ReadyUp();

    protected virtual void DeployMinigame(){
        GameObject.Find("Screens").transform.Find("Game Screen").gameObject.AddComponent<DeployableMinigame>();
    }

}
