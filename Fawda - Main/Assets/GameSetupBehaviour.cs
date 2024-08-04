using UnityEngine.Events;

public abstract class GameSetupBehaviour
{
    private bool[] readies;
    protected UnityAction<int, bool> rosterAction = null;

    public GameSetupBehaviour(){
        readies = new bool[5];
        DebugLogger.SourcedPrint("GameSetup (grandfather logic)", "Now accepting readies");
        UIManager.bannerUIBehaviour.ClearBannerMessage();
    }

    public virtual void ReadyUp() => LobbyManager.gameManager.StartGame();

    protected void TriggerMapLoad() => LobbyManager.gameManager.activeMinigame.LoadMap();

    protected void ChangeReadyStatus(byte[] _data, int _idx){
        DebugLogger.SourcedPrint("Game Setup (grandfather logic)", "logic tripped");

        bool newVal = new SimpleBooleanMessage(_data).ready;
        readies[_idx] = newVal;
        UpdateGlyphs(_idx, newVal);
        int cumCount = 0;
        foreach(bool r in readies) if (r) cumCount += 1;
        if (cumCount != LobbyManager.singleton.GetLobbySize())
            return;
        UIManager.bannerUIBehaviour.ClearBannerMessage();
        UIManager.bannerUIBehaviour.AddBannerMessage("Lets play", 0.5f);
        ReadyUp();
    }

    protected virtual void ResetReadies(){
        readies = new bool[LobbyManager.singleton.GetLobbySize()];
        for (int i = 0; i < readies.Length; i++){
            readies[i] = false;
        }
    }

    protected void UpdateGlyphs(int _idx, bool _ready)
    {
        if (rosterAction == null) UIManager.RosterManager.SetPlayerRosterBadgeVisibility(_idx, true, _colorHex: _ready?"#00FF00":"FFFFFF");
        
        else rosterAction(_idx, _ready);
    }
}
