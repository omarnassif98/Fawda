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
    public UnityEvent screenTransitionEvent = new UnityEvent();
    private Animator roomCodeAnimator;
    private TMP_Text roomCodeText;
    private GameObject startScreen;


    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton= this;
        RosterManager = new RosterUIBehaviour(transform.Find("Lobby Roster"));
        roomCodeAnimator = transform.Find("Room Code").GetComponent<Animator>();
        debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        startScreen = transform.Find("Screens").Find("Main Menu Screen").gameObject;
        roomCodeText = roomCodeAnimator.transform.Find("Room Code Text").GetComponent<TMP_Text>();

    }
    // Start is called before the first frame update
    void Start()
    {
        print("Hello?");
        ConnectionManager.singleton.RegisterServerEventListener("listen", UpdateRoomCode);
        ConnectionManager.singleton.RegisterServerEventListener("listen", () => SetRoomCodeVisibility(true));
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", () => ChangeScreen("Main Menu Screen"));
        LobbyManager.singleton.playerJoinEvent.AddListener(RosterManager.AddPlayerToRoster);

    }
    public void SwitchScreen(){
        screenTransitionEvent.Invoke();
    }


    public void SetRoomCodeVisibility(bool _newStatus){
        roomCodeAnimator.SetBool("visibility", _newStatus);
    }

    void UpdateRoomCode(){
        roomCodeText.text = ConnectionManager.singleton.GetRoomCode();
    }

    public void ChangeScreen(string _screenName){
        if(currentScreen != null){
            currentScreen.gameObject.SetActive(false);
        }
        ScreenManager newScreen = GameObject.Find("Screens").transform.Find(_screenName).GetComponent<ScreenManager>();
        newScreen.gameObject.SetActive(true);
        currentScreen = newScreen;
        MenuCursorManager.singleton.SetCursorInteractivities(true);
    }


    // Update is called once per frame

}
