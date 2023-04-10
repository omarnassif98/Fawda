using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager singleton;
      private void Awake(){
        if(singleton != null && singleton != this){
            Destroy(this);
        }else{
            singleton = this;
        }
    }
    
    public void Start(){
        ClientConnection.singleton.serverEvents["connect"].AddListener(kickoff);
    }

    [SerializeField] GameObject[] screens;
    public void kickoff(){
        screens[0].SetActive(false);
        screens[1].SetActive(true);
    }

    public void ManipulateMenu(int dir){
        ClientConnection.singleton.SendMessageToServer(OpCode.MENU_CONTROL, (byte)dir);
    }
}
