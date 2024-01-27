using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
public class RosterUIBehaviour
{
    public UnityEvent<int> rouletteDecisionEvent = new UnityEvent<int>();
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


    
    private PlayerLobbyRosterSlot[] rosterPlayers;
    private short occupationCount = 0;
    private Image rosterRoulleteTicker;
    public RosterUIBehaviour(Transform _rosterParent){
        
        rosterPlayers = new PlayerLobbyRosterSlot[_rosterParent.childCount];
        for(int i = 0; i <rosterPlayers.Length; i++){
            rosterPlayers[i] = new PlayerLobbyRosterSlot(_rosterParent.GetChild(i));
        }
        GameObject roulette = new GameObject();
        roulette.transform.parent = _rosterParent;
        rosterRoulleteTicker = roulette.AddComponent<Image>();
        rosterRoulleteTicker.rectTransform.sizeDelta = new Vector2(30,30);
        rosterRoulleteTicker.color = Color.red;
        TidyRoster();
    }

    public void AddPlayerToRoster(string _name, Color _color){
        if(occupationCount == rosterPlayers.Length){
            DebugLogger.singleton.Log("EYYO Trying to add player when roster is full");
            return;
        }
        rosterPlayers[occupationCount].occupied = true;
        rosterPlayers[occupationCount].playerColorImage.color = _color;
        rosterPlayers[occupationCount].playerNameText.text = _name;
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

    public void StartReadyupProcess(bool[] _participants){
        List<(Transform,int)> optedRosterSlots = new List<(Transform,int)>();
        for (int i = 0; i<_participants.Length; i++){
            if(_participants[i]){
                 optedRosterSlots.Add((GetRosterTransform(i),i));
                DebugLogger.singleton.Log(string.Format("Slot {0} opt in", i));
            }
        }
        if (optedRosterSlots.Count == 0)
        {
            DebugLogger.singleton.Log("Ummm...");
            for(int i = 0; i < _participants.Length; i++){
                optedRosterSlots.Add((GetRosterTransform(i),i));
            }   
        }
        UIManager.singleton.StartCoroutine(SlotRoulette(optedRosterSlots));
    }

    IEnumerator SlotRoulette(List<(Transform, int)> rosterTransforms){
        
        rosterRoulleteTicker.color = Color.red;
        if (rosterTransforms.Count == 1){
            SetTickerPosition(rosterTransforms[0].Item1);
            rouletteDecisionEvent.Invoke(rosterTransforms[0].Item2);
            yield break;
        }
        int cap = rosterTransforms.Count;
        int idx = Random.Range(0,cap);
        int dials = Random.Range(13,18);
        int turn = 0;
        float tickTime = 0.05f;
        while(turn <= dials){
            SetTickerPosition(rosterTransforms[idx].Item1);
            yield return new WaitForSeconds(tickTime);
            tickTime *= 1.2f;
            idx = (idx + 1) % cap;
            turn += 1;
        }
        yield return new WaitForSeconds(0.4f);
        rosterRoulleteTicker.color = Color.yellow;
        rouletteDecisionEvent.Invoke(rosterTransforms[idx].Item2);
    }

    private void SetTickerPosition(Transform slotTransform){
        rosterRoulleteTicker.rectTransform.position = slotTransform.position + (Vector3.up * 45);
    }
}
