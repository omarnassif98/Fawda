using System;
using System.Collections;
using System.Collections.Generic;

public abstract class SynapseDatastruct
{
    public abstract ArrayList PackData();
    public byte[] Encode(){
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
