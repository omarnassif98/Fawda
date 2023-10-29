using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] subscreens;
    private int currentSubscreenIdx = 0;

    public void SwitchSubscreens(int _newIdx){
        subscreens[currentSubscreenIdx].SetActive(false);
        subscreens[_newIdx].SetActive(true);
        currentSubscreenIdx = _newIdx;
    }

}
