using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public abstract class SynapseDatastruct
{
    public abstract ArrayList PackData();

    public virtual byte[] Encode(){
        byte[] bytes = SynapseMessageFormatter.GetPackedDataBytes(PackData());
        return bytes;
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
        this.name = System.Text.Encoding.ASCII.GetString(_data,0,12).Trim();
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

[System.Serializable]
public class GamepadData : SynapseDatastruct{
    public float xInput, yInput;
    public byte[] additionalInfo = new byte[0];


    public GamepadData(float _xInput, float _yInput){
        this.xInput = _xInput;
        this.yInput = _yInput;
    }

    public GamepadData(float _xInput, float _yInput, bool _action){
        this.xInput = _xInput;
        this.yInput = _yInput;
        this.additionalInfo = BitConverter.GetBytes(_action);
    }

    public GamepadData(byte[] _data){
        this.additionalInfo = new byte[_data.Length - 8];
        this.xInput = BitConverter.ToSingle(_data,0);
        this.yInput = BitConverter.ToSingle(_data,4);
        if(_data.Length - 8 == 0) return;
        Buffer.BlockCopy(_data, 8, this.additionalInfo, 0, _data.Length - 8);
    }

    public override ArrayList PackData(){
        ArrayList data = new ArrayList();
        data.Add(this.xInput);
        data.Add(this.yInput);
        return data;
    }
    public override byte[] Encode(){
        byte[] baseBytes = base.Encode();
        byte[] data = new byte[baseBytes.Length + additionalInfo.Length];
        Buffer.BlockCopy(baseBytes, 0, data, 0, baseBytes.Length);
        if (additionalInfo.Length == 0) return data;
        Buffer.BlockCopy(additionalInfo, 0, data, baseBytes.Length, additionalInfo.Length);
        return data;
    }
}

    public class SimpleBooleanMessage : SynapseDatastruct
    {
        public bool ready;
        public SimpleBooleanMessage(bool _ready){
            ready = _ready;
        }

        public SimpleBooleanMessage(byte[] _data){
            ready = BitConverter.ToBoolean(_data);
        }

        public override ArrayList PackData()
        {
            ArrayList allData = new ArrayList();
            allData.Add(ready);
            return allData;
        }
    }

    public class TransformSynchronizationMessage : SynapseDatastruct{
        public int entitySceneID;
        public Vector3 position;
        public float yRot;
        public TransformSynchronizationMessage(int _entitySceneID, Vector3 _position, float _yRot){
            entitySceneID = _entitySceneID;
            position = _position;
            yRot = _yRot;
        }


        public TransformSynchronizationMessage(byte[] _data){
            entitySceneID = _data[0];
            position = new Vector3(BitConverter.ToSingle(_data,1), BitConverter.ToSingle(_data,5), BitConverter.ToSingle(_data,9));
            yRot = BitConverter.ToSingle(_data,13);
        }

        public override ArrayList PackData()
        {
            ArrayList allData = new ArrayList();
            allData.Add(entitySceneID);
            allData.Add(position.X);
            allData.Add(position.Y);
            allData.Add(position.Z);
            allData.Add(yRot);
            return allData;
        }
    }
