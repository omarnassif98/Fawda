using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
public class UIManager : MonoBehaviour
{

    private ScreenManager currentScreen = null;
    public BackgroundBehaviour backgroundBehaviour;
    public static UIManager singleton;
    public static BannerUIBehaviour bannerUIBehaviour;
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private TMP_Text countdownText;

    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton= this;
        debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        RosterManager = new RosterUIBehaviour(transform.Find("Lobby Roster"));
        countdownText = transform.Find("Countdown").GetComponent<TMP_Text>();
        blackoutBehaviour = GameObject.FindObjectOfType<BlackoutBehaviour>();

    }
    // Start is called before the first frame update
    void Start()
    {

        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);
        bannerUIBehaviour = new BannerUIBehaviour(transform.Find("Banner").Find("Text").GetComponent<TMP_Text>());

    }



    public void SetCountdown(int _digits){
        countdownText.text = _digits.ToString();
    }

  
}
