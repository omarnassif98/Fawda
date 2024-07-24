using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{

    private ScreenManager currentScreen = null;
    public BackgroundBehaviour backgroundBehaviour;
    public static UIManager singleton;
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private TMP_Text countdownText, bannerText;
    List<string> bannerMessages;
    int bannerMessageIdx = -1;
    IEnumerator messageCycle;

    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton= this;
        debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        RosterManager = new RosterUIBehaviour(transform.Find("Lobby Roster"));
        countdownText = transform.Find("Countdown").GetComponent<TMP_Text>();
        blackoutBehaviour = GameObject.FindObjectOfType<BlackoutBehaviour>();
        bannerText = transform.Find("Banner").Find("Text").GetComponent<TMP_Text>();
        bannerMessages = new List<string>();

    }
    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage(ConnectionManager.singleton.GetRoomCode()); });
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage("Join Now"); });
        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);

    }



    public void SetCountdown(int _digits){
        countdownText.text = _digits.ToString();
    }





    public void AddBannerMessage(string _message)
    {
        bannerMessages.Add(_message);
        RecalculateBannerVisibility();
    }

    public void RemoveBannerMessage(string _message) {
        bannerMessages.Remove(_message);
        RecalculateBannerVisibility();
    }

    public void ClearBannerMessage()
    {
        bannerMessages.Clear();
        RecalculateBannerVisibility();
    }

    void RecalculateBannerVisibility()
    {
        bannerText.transform.parent.GetComponent<Animator>().SetBool("visibility", bannerMessages.Count > 0);
        bannerMessageIdx = Mathf.Min(bannerMessageIdx, bannerMessages.Count);
        if(messageCycle != null && bannerMessages.Count == 0)
        {
            StopCoroutine(messageCycle);
            messageCycle = null;
            return;
        }else if(messageCycle == null)
        {
            messageCycle = CycleBannerMessageIdx();
            StartCoroutine(messageCycle);
        }
    }


    IEnumerator CycleBannerMessageIdx()
    {
        DebugLogger.SourcedPrint("Banner", "cycle");
        bannerMessageIdx = (bannerMessageIdx + 1) % bannerMessages.Count;
        bannerText.text = bannerMessages[bannerMessageIdx];
        yield return new WaitForSeconds(3);
        messageCycle = CycleBannerMessageIdx();
        StartCoroutine(messageCycle);
    }
    // Update is called once per frame

}
