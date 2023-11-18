using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public BackgroundBehaviour backgroundBehaviour;

    public static UIManager singleton;
    public static DebugSystemsManager debugSystems;
    

    [SerializeField]
    private short currentScreen = 0;

    [SerializeField]
    private TMP_Text room_code_text;
    public UnityEvent screenTransitionEvent = new UnityEvent();
   
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
    }
    public void SwitchScreen(){
        screenTransitionEvent.Invoke();
    }








    // Update is called once per frame

}
