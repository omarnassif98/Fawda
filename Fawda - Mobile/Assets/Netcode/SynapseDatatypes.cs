using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public abstract class SynapseDatastruct
{
    public abstract ArrayList PackData();
    public virtual byte[] Encode(){
        return SynapseMessageFormatter.GetPackedDataBytes(PackData());
    }
}




[System.Serializable]
public class ProfileData : SynapseDatastruct{
    public string name;
    public int colorSelection;
    public int topCustomization;
    public int midCustomization;
    public int botCustomization;

    public ProfileData(string _name, int _colorSelection){
        this.name = _name;
        this.colorSelection = _colorSelection;
        this.topCustomization = -1;
        this.midCustomization = -1;
        this.botCustomization = -1;
    }

    public ProfileData(byte[] _data){
        this.name = BitConverter.ToString(_data,0,12).Trim();
        this.colorSelection = BitConverter.ToInt32(_data,12);
        this.topCustomization = _data[13];
        this.midCustomization = _data[14];
        this.botCustomization = _data[15];
        

    }

    public override ArrayList PackData(){
        ArrayList data = new ArrayList();
        data.Add(this.name);
        data.Add(this.colorSelection);
        data.Add(this.topCustomization);
        data.Add(this.midCustomization);
        data.Add(this.botCustomization);
        return data;
    }


}

public enum InputType{
    Menu = OpCode.MENU_CONTROL,
    Joypad = OpCode.UDP_GAMEPAD_INPUT
} 

[System.Serializable]
public class GamepadData : SynapseDatastruct{
    public float dir, dist;
    public byte[] additionalInfo = new byte[0];


    public GamepadData(float _dir, float _dist){
        this.dir = _dir;
        this.dist = _dist;
    }

    public GamepadData(byte[] _data){
        this.dir = BitConverter.ToSingle(_data,0);
        this.dist = BitConverter.ToSingle(_data,4);
        if(_data.Length - 8 == 0) return;
        Buffer.BlockCopy(_data, 8, this.additionalInfo, 0, _data.Length - 8);
    }

    public override ArrayList PackData(){
        ArrayList data = new ArrayList();
        data.Add(this.dir);
        data.Add(this.dist);      
        return data;
    }

    public override byte[] Encode(){
        byte[] baseBytes = base.Encode();
        if(additionalInfo.Length == 0) return baseBytes;
        byte[] fullBytes = new byte[baseBytes.Length + additionalInfo.Length];
        Buffer.BlockCopy(baseBytes,0,fullBytes,0,baseBytes.Length);
        Buffer.BlockCopy(additionalInfo,0,fullBytes,baseBytes.Length,additionalInfo.Length);
        return fullBytes;
    }


}