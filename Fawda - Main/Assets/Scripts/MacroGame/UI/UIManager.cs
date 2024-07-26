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
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private TMP_Text countdownText, bannerText;
    int bannerMessageIdx = -1;
    IEnumerator messageCycle, flashEvent;
    Dictionary<string, bannerWord> bannerMessages = new Dictionary<string, bannerWord>();

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

    }
    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage(ConnectionManager.singleton.GetRoomCode()); });
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => { AddBannerMessage("Join Now"); });
        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => FlashMessage("Lets Get This Party Started", 0.25f));

    }



    public void SetCountdown(int _digits){
        countdownText.text = _digits.ToString();
    }

    public void AddBannerMessage(string _message, float _flashTime = 3)
    {
        bannerMessages[_message] = new bannerWord(_message, _flashTime);
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
        }else if(messageCycle == null && flashEvent == null)
        {
            messageCycle = CycleBannerMessageIdx();
            StartCoroutine(messageCycle);
        }
    }


    IEnumerator CycleBannerMessageIdx()
    {
        DebugLogger.SourcedPrint("Banner", "cycle");
        bannerMessageIdx = (bannerMessageIdx + 1) % bannerMessages.Count;
        bannerText.text = bannerMessages.ElementAt(bannerMessageIdx).Value.word;
        yield return new WaitForSeconds(bannerMessages.ElementAt(bannerMessageIdx).Value.flashTime);
        messageCycle = CycleBannerMessageIdx();
        StartCoroutine(messageCycle);
    }

    Queue<bannerWord> flashQueue = new Queue<bannerWord>();

    public void FlashMessage(string _message, float _flashTime = 5)
    {
        foreach (string word in _message.Split(' ')) flashQueue.Enqueue(new bannerWord(word, _flashTime));
        if (flashEvent != null) return;
        flashEvent = EmptyFlashQueue();
        StartCoroutine(flashEvent);
    }

    struct bannerWord
    {
        public string word;
        public float flashTime;
        public bannerWord(string _word, float _flashTime)
        {
            word = _word;
            flashTime = _flashTime;
        }

    }
    IEnumerator EmptyFlashQueue()
    {
        if (messageCycle != null)
        {
            StopCoroutine(messageCycle);
            messageCycle = null;
        }

        while(flashQueue.Count > 0)
        {
            bannerWord currentWord = flashQueue.Dequeue();
            bannerText.text = currentWord.word;
            yield return new WaitForSeconds(currentWord.flashTime);
        }
        flashEvent = null;
        messageCycle = CycleBannerMessageIdx();
        StartCoroutine(messageCycle);

    }
    // Update is called once per frame

}
