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
    public static BannerUIBehaviour bannerUIBehaviour;
    public static DebugSystemsManager debugSystems;
    public static RosterUIBehaviour RosterManager;
    public static BlackoutBehaviour blackoutBehaviour;
    private TMP_Text BigText;
    private Image indicator;
    private float indicatorLifespan, indicatorExpiry;

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

    }
    // Start is called before the first frame update
    void Start()
    {

        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);
        bannerUIBehaviour = new BannerUIBehaviour(transform.Find("Banner").Find("Text").GetComponent<TMP_Text>());

    }


    public void SetCountdown(int _digits) => SetCountdown(_digits.ToString());

    public void SetCountdown(string _message, float? _expiry = null, Color? _color = null){
        BigText.gameObject.SetActive(true);
        BigText.text = _message;
        if (_color != null) BigText.color = (Color)_color;
        if (_expiry == null) return;
        Vector3 indicatorPos = BigText.textBounds.max;
        indicator.transform.position = indicatorPos;
        indicator.gameObject.SetActive(true);
        indicatorLifespan = (float)_expiry;
        indicatorExpiry = indicatorLifespan;
    }

    public float GetScale()
    {
        return Screen.width / transform.parent.GetComponent<CanvasScaler>().referenceResolution.x;
    }
  
}
