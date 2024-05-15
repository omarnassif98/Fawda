using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    private ScreenManager currentScreen = null;
    public BackgroundBehaviour backgroundBehaviour;
    public static UIManager singleton;
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private Animator roomCodeAnimator;
    private TMP_Text roomCodeText, countdownText;


    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton= this;
        debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        RosterManager = new RosterUIBehaviour(transform.Find("Lobby Roster"));
        roomCodeAnimator = transform.Find("Room Code").GetComponent<Animator>();
        countdownText = transform.Find("Countdown").GetComponent<TMP_Text>();
        roomCodeText = roomCodeAnimator.transform.Find("Room Code Text").GetComponent<TMP_Text>();
        blackoutBehaviour = GameObject.FindObjectOfType<BlackoutBehaviour>();

    }
    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.singleton.RegisterServerEventListener("listen", UpdateRoomCode);
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => SetRoomCodeVisibility(true));
        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);

    }


    public void SetRoomCodeVisibility(bool _newStatus){
        roomCodeAnimator.SetBool("visibility", _newStatus);
    }

    void UpdateRoomCode(){
        roomCodeText.text = ConnectionManager.singleton.GetRoomCode();
    }


    public void SetCountdown(int _digits){
        countdownText.text = _digits.ToString();
    }

    // Update is called once per frame

}
