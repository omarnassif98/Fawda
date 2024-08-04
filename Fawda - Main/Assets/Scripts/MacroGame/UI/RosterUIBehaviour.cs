using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class RosterUIBehaviour
{

    //Self managed class for the actual slot, one per player
    class PlayerLobbyRosterSlot{
        public TMP_Text playerNameText;
        public Image playerColorImage;
        public Animator rosterAnimator;
        public Image badge;
        public Image badgeGlyph;
        public TMP_Text badgeText;
        public bool occupied;
        public int lobbyIdx;

        public PlayerLobbyRosterSlot(Transform _rosterSlot){
            playerColorImage = _rosterSlot.Find("Backdrop").Find("Player Color").GetComponent<Image>();
            playerNameText = playerColorImage.transform.Find("Player Name").GetComponent<TMP_Text>();
            rosterAnimator = _rosterSlot.GetComponent<Animator>();
            badge = _rosterSlot.Find("Badge").GetComponent<Image>();
            badgeText = badge.transform.Find("Badge Text").GetComponent<TMP_Text>();
            badgeGlyph = badge.transform.Find("Glyph").GetComponent<Image>();
            badge.gameObject.SetActive(false);
            badgeText.gameObject.SetActive(false);
            badgeGlyph.gameObject.SetActive(false);
            occupied = false;
            lobbyIdx = -1;
        }

        public void Import(PlayerLobbyRosterSlot _rosterSlot){
            playerColorImage.color = _rosterSlot.playerColorImage.color;
            playerNameText.text = _rosterSlot.playerNameText.text;
            rosterAnimator.SetBool("Revealed", _rosterSlot.rosterAnimator.GetBool("Revealed"));
            badge.gameObject.SetActive(_rosterSlot.badge.IsActive());
            badgeText.text = _rosterSlot.badgeText.text;
            occupied = _rosterSlot.occupied;
            rosterAnimator.SetBool("Occupied", occupied);
            lobbyIdx = _rosterSlot.lobbyIdx;
        }

        public void Occupy(int _idx)
        {
            SetRosterSlotOccupationStatus(true);
            playerColorImage.color = ResourceManager.namedColors[LobbyManager.players[_idx].colorSelection].color;
            playerNameText.text = LobbyManager.players[_idx].name;
            lobbyIdx = _idx;
        }

        public void Relieve() => SetRosterSlotOccupationStatus(false);

        public void SetRosterStatus(bool _visibility, string _colorHex)
        {
            badge.gameObject.SetActive(_visibility);
            Color badgeColor = new Color();
            ColorUtility.TryParseHtmlString(_colorHex, out badgeColor);
            badge.color = badgeColor;
            badgeText.gameObject.SetActive(false);
            badgeGlyph.gameObject.SetActive(false);
        }

        public void SetRosterStatus(bool _visibility, string _txt, string _colorHex)
        {
            SetRosterStatus(_visibility, _colorHex);
            badgeText.text = _txt;
            badgeText.gameObject.SetActive(true);
        }

        public void SetRosterStatus(bool _visibility, Sprite _sprite, string _colorHex)
        {
            DebugLogger.SourcedPrint("Roster", "Trying Glyph");
            SetRosterStatus(_visibility, _colorHex);
            badgeGlyph.sprite = _sprite;
            badgeGlyph.gameObject.SetActive(true);
        }


        private void SetRosterSlotOccupationStatus(bool _occupationStatus)
        {
            rosterAnimator.SetBool("Occupied", _occupationStatus);
            occupied = _occupationStatus;
        }

        public void SetRosterSlotVisibility(bool _visibility) => rosterAnimator.SetBool("Revealed", _visibility);

    }


    private PlayerLobbyRosterSlot[] rosterPlayers;
    private short occupationCount = 0;
    public UnityEvent<int> rosterEvent;


    //The roullete is this randomized way of selecting one random player
    //It's randomized with a variable length
    //It has a callback... somewhere 
    private TMP_Text rosterRoulleteTicker;
    IEnumerator roulettePlaying;

    //Context setting + ticker spawn
    public RosterUIBehaviour(Transform _rosterParent){
        rosterEvent = new UnityEvent<int>();
        rosterPlayers = new PlayerLobbyRosterSlot[_rosterParent.childCount];
        for(int i = 0; i <rosterPlayers.Length; i++) rosterPlayers[i] = new PlayerLobbyRosterSlot(_rosterParent.GetChild(i));
        GameObject roulette = new GameObject();
        roulette.transform.parent = _rosterParent;
        rosterRoulleteTicker = roulette.AddComponent<TextMeshProUGUI>();
        rosterRoulleteTicker.text = "▼";
        rosterRoulleteTicker.rectTransform.sizeDelta = new Vector2(15,15);
        rosterRoulleteTicker.color = Color.red;
        rosterRoulleteTicker.alignment = TextAlignmentOptions.Center;
        roulette.name = "Ticker";
        rosterRoulleteTicker.rectTransform.localScale = Vector3.one;
        TidyRoster();
    }

    //Add to first available roster, which will always be the idx of the size of the lobby
    public void AddPlayerToRoster(int _idx){
        foreach(PlayerLobbyRosterSlot lobbyRosterSlot in rosterPlayers)
        {
            if (!lobbyRosterSlot.occupied)
            {
                lobbyRosterSlot.Occupy(_idx);
                occupationCount += 1;
                break;
            }
        }
        TidyRoster();
    }

    public void RemovePlayerFromRoster(short _idx){
        if(!rosterPlayers[_idx].occupied){
            DebugLogger.singleton.Log(string.Format("EYYO slot {0} is already empty", _idx));
            return;
        }
        rosterPlayers[_idx].occupied = false;
        occupationCount -= 1;
        TidyRoster();
    }

    public void SetPlayerRosterBadgeVisibility(int _idx, bool _visibility, Sprite _sprite = null, string _txt = "", string _colorHex = "#FFFFFF") {
        if (_sprite != null) rosterPlayers[_idx].SetRosterStatus(_visibility, _sprite, _colorHex);
        else if (_txt.Length > 0) rosterPlayers[_idx].SetRosterStatus(_visibility, _txt, _colorHex);
        else rosterPlayers[_idx].SetRosterStatus(_visibility, _colorHex);
    }

    //The roster is ALWAYS gonna be continous from left to right

    //This process will make it so that even if in the lobby players 2 and 3 are in
    // the roster would show slots 1 and 2 taken respectively (lobbyIdx is important) 
    private void TidyRoster(){
        DebugLogger.SourcedPrint("Roster", string.Format("occupation count - {0}", occupationCount));
        short[] newIdxs = new short[occupationCount];
        short currIdx = 0;
        for(short i = 0; i < rosterPlayers.Length; i++){
            if(rosterPlayers[i].occupied){
                newIdxs[currIdx] = i;
                currIdx += 1;
            }
        }

        for (short i = 0; i < occupationCount; i++) rosterPlayers[i].Import(rosterPlayers[newIdxs[i]]);

        for(short i = occupationCount; i < rosterPlayers.Length; i++){
            rosterPlayers[i].Relieve();
            rosterPlayers[i].SetRosterSlotVisibility(i == occupationCount);
        }
    }




    // # Roulette

    public void StartRoulette(){
        DebugLogger.SourcedPrint("Roster Roulette", "Preprocessing");
        FlushRoulette();
        roulettePlaying = SlotRoulette();
        UIManager.singleton.StartCoroutine(roulettePlaying);
    }

    IEnumerator SlotRoulette(){
        DebugLogger.SourcedPrint("Roster Roulette", "Start");
        rosterRoulleteTicker.enabled = true;
        rosterRoulleteTicker.color = Color.red;
        if (occupationCount == 1){
            SetTickerPosition(rosterPlayers[0].rosterAnimator.transform);
            DebugLogger.SourcedPrint("Roster Roulette", "Just 1?");
            rosterRoulleteTicker.color = Color.yellow;
            rosterEvent.Invoke(0);
            DismissRoulette();
            yield break;
        }

        int idx = Random.Range(0,occupationCount);
        int dials = Random.Range(13,18);
        int turn = 0;
        float tickTime = 0.05f;
        while(turn <= dials){
            idx = (idx + 1) % occupationCount;
            SetTickerPosition(rosterPlayers[idx].rosterAnimator.transform);
            yield return new WaitForSeconds(tickTime);
            tickTime *= 1.2f;
            turn += 1;
        }
        DebugLogger.SourcedPrint("Roster Roulette", "Stopped");
        yield return new WaitForSeconds(0.4f);
        DebugLogger.SourcedPrint("Roster Roulette", "Chosen", ColorUtility.ToHtmlStringRGB(rosterPlayers[idx].playerColorImage.color));
        rosterRoulleteTicker.color = Color.yellow;
        yield return new WaitForSeconds(0.2f);

        rosterEvent.Invoke(idx);
        DismissRoulette();
    }

    public void DismissRoulette(){
        FlushRoulette();
        rosterEvent.RemoveAllListeners();
    }

    private void FlushRoulette(){
        if(roulettePlaying == null) return;
        DebugLogger.SourcedPrint("Roster Roulette", "Clearing junk");
        UIManager.singleton.StopCoroutine(roulettePlaying);
        roulettePlaying = null;

    }

    private void SetTickerPosition(Transform slotTransform)
    {
        rosterRoulleteTicker.rectTransform.position = slotTransform.position + (Vector3.up * 37.5f * UIManager.singleton.GetScale());
    }

    public void SetTickerPosition(int _idx)
    {
        SetTickerPosition(rosterPlayers[_idx].rosterAnimator.transform);
    }
}