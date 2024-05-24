using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] subscreens;
    protected int currentSubscreenIdx = 0;

    public void SwitchSubscreens(int _newIdx){
        subscreens[currentSubscreenIdx].SetActive(false);
        subscreens[_newIdx].SetActive(true);
        currentSubscreenIdx = _newIdx;
    }

}
