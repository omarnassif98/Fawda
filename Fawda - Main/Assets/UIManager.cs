using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{

    public static UIManager singleton;
    public static DebugSystemsManager debugSystems;
    [SerializeField]
    private ScreenManager[] screens;
    [SerializeField]
    private short currentScreen = 0;

    [SerializeField]
    private TMP_Text room_code_text;
    public UnityEvent screenFillEvent = new UnityEvent();
    public UnityEvent screenClearEvent = new UnityEvent();
    private bool navLock = true;
    private Stack<short> screenStack = new Stack<short>();
    void Awake(){
        if(singleton == null){
            singleton= this;
            debugSystems = gameObject.GetComponent<DebugSystemsManager>();
        }else{
            Destroy(this);
        }
    }

    void UpdateRoomCode(){
        room_code_text.text = ConnectionManager.singleton.GetRoomCode();
    }
    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.singleton.RegisterServerEventListener("listen", UpdateRoomCode);
        ConnectionManager.singleton.RegisterServerEventListener("wakeup", FillScreen);
        screenClearEvent.AddListener(FillScreen);
        screenFillEvent.AddListener(onScreenFill);
    }

    public void SwitchScreens(short _newidx, bool _push = true){
        if(_newidx == -1) return;
        if(_push) screenStack.Push(currentScreen);
        screens[currentScreen].DismissScreen();
        screens[_newidx].gameObject.SetActive(true);
        currentScreen = _newidx;
    }

    public void PopScreen(){
        if(screenStack.Count == 0) return;
        short prev = screenStack.Pop();
        SwitchScreens(prev, false);
    }

    private void FillScreen(){
        screens[currentScreen].IntroduceScreen();
        navLock = true;
    }

    private void onScreenFill(){
        navLock = false;
    }




    // Update is called once per frame

}
