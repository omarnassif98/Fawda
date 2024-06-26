using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
public class RosterUIBehaviour
{
    struct PlayerLobbyRosterSlot{
        public TMP_Text playerNameText;
        public Image playerColorImage;
        public Animator rosterAnimator;
        public bool occupied;
        public PlayerLobbyRosterSlot(Transform _rosterSlot){
            playerColorImage = _rosterSlot.Find("Backdrop").Find("Player Color").GetComponent<Image>();
            playerNameText = playerColorImage.transform.Find("Player Name").GetComponent<TMP_Text>();
            rosterAnimator = _rosterSlot.GetComponent<Animator>();
            occupied = false;
        }
    }

    IEnumerator roulettePlaying;

    private PlayerLobbyRosterSlot[] rosterPlayers;
    private short occupationCount = 0;
    private TMP_Text rosterRoulleteTicker;
    public RosterUIBehaviour(Transform _rosterParent){
        rosterPlayers = new PlayerLobbyRosterSlot[_rosterParent.childCount];
        for(int i = 0; i <rosterPlayers.Length; i++){
            rosterPlayers[i] = new PlayerLobbyRosterSlot(_rosterParent.GetChild(i));
        }
        GameObject roulette = new GameObject();
        roulette.transform.parent = _rosterParent;
        rosterRoulleteTicker = roulette.AddComponent<TextMeshProUGUI>();
        rosterRoulleteTicker.text = "▼";
        rosterRoulleteTicker.rectTransform.sizeDelta = new Vector2(15,15);
        rosterRoulleteTicker.color = Color.red;
        roulette.name = "Ticker";
        rosterRoulleteTicker.enabled = false;
        TidyRoster();
    }

    public void AddPlayerToRoster(int _idx){
        if(occupationCount == rosterPlayers.Length){
            DebugLogger.singleton.Log("EYYO Trying to add player when roster is full");
            return;
        }
        rosterPlayers[occupationCount].occupied = true;
        rosterPlayers[occupationCount].playerColorImage.color = ResourceManager.namedColors[LobbyManager.players[_idx].colorSelection].color;
        rosterPlayers[occupationCount].playerNameText.text = LobbyManager.players[_idx].name;
        occupationCount += 1;
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


    public Transform GetRosterTransform(int _index){
        return rosterPlayers[_index].rosterAnimator.transform;
    }

    private void TidyRoster(){
        short[] newIdxs = new short[occupationCount];
        short currIdx = 0;
        for(short i = 0; i < rosterPlayers.Length && currIdx < newIdxs.Length; i++){
            if(rosterPlayers[i].occupied){
                newIdxs[currIdx] = i;
                currIdx += 1;
            }
        }

        for(short i = 0; i < occupationCount; i++){
            rosterPlayers[i].occupied = true;
            rosterPlayers[i].playerColorImage.color = rosterPlayers[newIdxs[i]].playerColorImage.color;
            rosterPlayers[i].playerNameText.text = rosterPlayers[newIdxs[i]].playerNameText.text;
            SetRosterSlotOccupationStatus(i, true);
            SetRosterSlotVisibility(i,true);
        }

        for(short i = occupationCount; i < rosterPlayers.Length; i++){
            rosterPlayers[i].occupied = false;
            SetRosterSlotOccupationStatus(i, false);
            SetRosterSlotVisibility(i,i == occupationCount);
        }
        DebugLogger.singleton.Log(string.Format("Tidied roster, occCount is {0}", occupationCount));

    }

    private void SetRosterSlotVisibility(int _idx, bool _visibility){
        rosterPlayers[_idx].rosterAnimator.SetBool("Revealed", _visibility);
    }

    private void SetRosterSlotOccupationStatus(int _idx, bool _occupationStatus){
        rosterPlayers[_idx].rosterAnimator.SetBool("Occupied", _occupationStatus);
        rosterPlayers[_idx].occupied = _occupationStatus;
    }

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

        roulettePlaying = null;
    }

    public void DismissRoulette(){
        FlushRoulette();
        rosterRoulleteTicker.enabled = false;
    }

    private void FlushRoulette(){
        if(roulettePlaying == null) return;
        DebugLogger.SourcedPrint("Roster Roulette", "Clearing junk");
        UIManager.singleton.StopCoroutine(roulettePlaying);
        roulettePlaying = null;

    }

    private void SetTickerPosition(Transform slotTransform){
        rosterRoulleteTicker.rectTransform.position = slotTransform.position + (Vector3.up * 90);
    }

}
