using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    
    private PlayerLobbyRosterSlot[] rosterPlayers;
    private short occupationCount = 0;
    
    public RosterUIBehaviour(Transform _rosterParent){
        
        rosterPlayers = new PlayerLobbyRosterSlot[_rosterParent.childCount];
        for(int i = 0; i <rosterPlayers.Length; i++){
            rosterPlayers[i] = new PlayerLobbyRosterSlot(_rosterParent.GetChild(i));
        }
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
}
