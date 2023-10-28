using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployableMinigame : MonoBehaviour
{
    
     public virtual void SetupGame(){
        print("Impliment Setup plz");
    }

    public virtual void EndGame(){
        print("Impliment Game End Situation plz");
    }

}
