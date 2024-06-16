using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ScreenManager
{
    private Transform[] subscreens;
    protected int currentSubscreenIdx = 0;

    public ScreenManager(Transform _transform){
        subscreens = new Transform[_transform.childCount];
        for(int i = 0; i < subscreens.Length; i++) subscreens[i] = _transform.GetChild(i);
    }

    public void SwitchSubscreens(int _newIdx){
        subscreens[currentSubscreenIdx].gameObject.SetActive(false);
        subscreens[_newIdx].gameObject.SetActive(true);
        currentSubscreenIdx = _newIdx;
    }

}
