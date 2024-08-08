using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    struct BigTextMessage
    {
        public string message;
        public float expiry;
        public Color? color;
        public bool showIndicator;
        public UnityAction callback;
        public BigTextMessage(string _message, float _expiry, Color? _color, bool _showIndicator, UnityAction _callback)
        {
            message = _message;
            expiry = _expiry;
            color = _color;
            showIndicator = _showIndicator;
            callback = _callback;
        }
    }

    private ScreenManager currentScreen = null;
    public BackgroundBehaviour backgroundBehaviour;
    public static UIManager singleton;
    public static BannerUIBehaviour bannerUIBehaviour;
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private TMP_Text BigText;
    private Image indicator;
    private float indicatorLifespan, indicatorExpiry;
    private Queue<BigTextMessage> messageQueue;
    private BigTextMessage currentMessage;
    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton= this;
        debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        RosterManager = new RosterUIBehaviour(transform.Find("Lobby Roster"));
        BigText = transform.Find("Overlay/BigText").GetComponent<TMP_Text>();
        indicator = transform.Find("Overlay/Indicator").GetComponent<Image>();
        blackoutBehaviour = GameObject.FindObjectOfType<BlackoutBehaviour>();
        messageQueue = new Queue<BigTextMessage>();
    }
    // Start is called before the first frame update
    void Start()
    {

        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);
        bannerUIBehaviour = new BannerUIBehaviour(transform.Find("Banner").Find("Text").GetComponent<TMP_Text>());

    }


    public void SetCountdown(int _digits) => SetCountdown(_digits.ToString());

    public void SetCountdown(string _message, float _expiry = 1, Color? _color = null, bool _showIndicator = false, UnityAction _callback = null) => messageQueue.Enqueue(new BigTextMessage(_message, _expiry, _color, _showIndicator, _callback));

    public float GetScale()
    {
        return Screen.width / transform.parent.GetComponent<CanvasScaler>().referenceResolution.x;
    }


    private void AdvanceMessageQueue()
    {
        if (messageQueue.Count == 0)
        {
            indicator.gameObject.SetActive(false);
            BigText.gameObject.SetActive(false);
            return;
        }
        currentMessage = messageQueue.Dequeue();
        BigText.gameObject.SetActive(true);
        BigText.text = currentMessage.message;
        BigText.color = (currentMessage.color != null)?(Color)currentMessage.color:Color.white;
        indicatorLifespan = currentMessage.expiry;
        indicatorExpiry = indicatorLifespan;
        BigText.ForceMeshUpdate();
        Vector3 indicatorPos = BigText.textBounds.max + new Vector3(10, -20, 0);
        indicator.transform.localPosition = indicatorPos;
        indicator.gameObject.SetActive(currentMessage.showIndicator);
    }

    private void Update()
    {
        if (indicatorLifespan <= 0)
        {
            if (currentMessage.callback != null) currentMessage.callback();
            currentMessage.callback = null;
            AdvanceMessageQueue();
            return;
        }

        if (indicatorExpiry <= 0)
        {
            indicatorLifespan = -1;
            return;
        }
        indicatorExpiry -= Time.deltaTime;
        indicator.fillAmount = Mathf.Max((indicatorExpiry / indicatorLifespan) - 0.1f, 0);
    }


}