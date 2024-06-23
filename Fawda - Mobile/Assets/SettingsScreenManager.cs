using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SettingsScreenManager : ScreenManager
{

    public SettingsScreenManager(Transform _transform) : base(_transform)
    {
        _transform.Find("MainSubscreen/DebugSettings").GetComponent<Button>().onClick.AddListener(() => SwitchSubscreens(1));
        _transform.Find("DebugSubscreen/BackButton").GetComponent<Button>().onClick.AddListener(() => SwitchSubscreens(0));
        _transform.Find("DebugSubscreen/ResetButton").GetComponent<Button>().onClick.AddListener(() => {
            Transform modal = Orchestrator.singleton.menuUIHandler.SummonModal("ResetDataModalScreen");
            modal.Find("ResetDataModalScreen/RESET").GetComponent<Button>().onClick.AddListener(Orchestrator.singleton.ResetData);
            });
    }
}
