using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntGameSetupBehaviour : PromptedGameSetupBehaviour
{
    Sprite ghostGlyph, hunterGlyph;
    public HauntGameSetupBehaviour() : base()
    {
        DebugLogger.SourcedPrint("Haunt Setup", "Setup", _color:"00FF00");
        ghostGlyph = Resources.Load<Sprite>("Global/UI/ghost");
        hunterGlyph = Resources.Load<Sprite>("Global/UI/camera");
        DebugLogger.SourcedPrint("Haunt Setup", string.Format("Glyph load = {0}", ghostGlyph != null), _color: "00FF00");
        rosterAction = (_idx, _ready) => UIManager.RosterManager.SetPlayerRosterBadgeVisibility(_idx, true, _colorHex: _ready ? "#00FF00" : "#FFFFFF", _sprite: _idx == promptIdx ? ghostGlyph : hunterGlyph);

    }

    public override void ReadyUp()
    {
        DebugLogger.SourcedPrint("HauntGameSetup","All Ready");
        ConnectionManager.singleton.VacateRPC(OpCode.READYUP);
        base.ReadyUp();
    }
}
