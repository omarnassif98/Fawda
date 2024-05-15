using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursorManager : MonoBehaviour
{
    public static MenuCursorManager singleton;
    private MenuCursorBehaviour[] cursors;
    void Awake(){
        if(singleton != null){
            Destroy(this);
        }
        singleton = this;
        cursors = new MenuCursorBehaviour[transform.childCount];
        for(short i = 0; i < cursors.Length; i++){
            cursors[i] = transform.GetChild(i).GetComponent<MenuCursorBehaviour>();
        }
    }

    void Start()
    {
        ConnectionManager.singleton.RegisterRPC(OpCode.MENU_OCCUPY, HandleClientOccupation);
    }

    public void UpdateCursorPlayer(ProfileData _player, int _idx){
        cursors[_idx].UpdateGraphics(_player.name, Color.gray); //_player.colorSelection
    }

    public void SetCursorInteractivities(bool _newStatus){
        for(int i = 0; i < cursors.Length; i++){
            cursors[i].ToggleCursorInteractivity(_newStatus);
        }
    }

    private void HandleClientOccupation(byte[] _data, int _idx){
        bool occ = BitConverter.ToBoolean(_data,0);
        cursors[_idx].HandleOccupation(occ);
    }

    private void ControlCursor(byte[] _data, int _idx){
        float dirInput = BitConverter.ToSingle(_data,0);
        float distInput = BitConverter.ToSingle(_data,4);
        cursors[_idx].UpdateJoypad(dirInput,distInput);
    }
}
