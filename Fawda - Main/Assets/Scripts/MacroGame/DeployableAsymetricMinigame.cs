using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//These are minigames which have follow an asymetric design (haunt, chase)
public abstract class DeployableAsymetricMinigame : DeployableMinigame {
    public int asymetricPlayerIdx;
    public virtual void RegisterAsymetricPlayer(int _specialityPlayer){
        DebugLogger.SourcedPrint("DeployableAsymetricGame", string.Format("Assymetric idx: {0}",_specialityPlayer));
        asymetricPlayerIdx = _specialityPlayer;
    }

}
