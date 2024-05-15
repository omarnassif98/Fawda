using System;
using System.CodeDom;
using System.Collections;
using System.Text;
using UnityEngine;

public static class SynapseMessageFormatter
{
    public static byte[] EncodeMessage(NetMessage _netMessage){
        byte[] msg = new byte[_netMessage.size + 2];
        msg[0] = (byte)_netMessage.size;
        msg[1] = (byte)_netMessage.opCode;
        for(short i = 0; i < _netMessage.size; i++){
            msg[i+2] = _netMessage.val[i];
        }
        return msg;
    }

    public static byte[] GetPackedDataBytes(ArrayList _packedData){
        int length = 0, lastIdx = 0;
        for(int i=0; i<_packedData.Count; i++){

           switch (Type.GetTypeCode(_packedData[i].GetType()))
           {
            case TypeCode.String:

                length += 12;
                break;
                    //Strings are to be 12 letters or less
            case TypeCode.Int32:
            case TypeCode.Int16:
            case TypeCode.Boolean:
                length += 1;
                break;
            case TypeCode.Single:
                length += 4;
            break;
           }
        }

        byte[] resBytes = new byte[length];
        for(int i=0; i<_packedData.Count; i++){
           switch (Type.GetTypeCode(_packedData[i].GetType()))
           {
            case TypeCode.String:
                string fixedString = ((string)_packedData[i]).PadRight(12).Substring(0,12);
                byte[] stringBytes = System.Text.Encoding.ASCII.GetBytes(fixedString);
                Buffer.BlockCopy(stringBytes,0,resBytes,lastIdx,stringBytes.Length);
                lastIdx += stringBytes.Length;
                break;
                    //Strings are to be 9 letters or less
            case TypeCode.Int32:
            case TypeCode.Int16:
                byte val = (byte)Mathf.Clamp((int)_packedData[i],0,255);
                resBytes[lastIdx] = val;
                lastIdx += 1;
                break;
                //All ints are 1 byte long, sue me
            case TypeCode.Single:
                byte[] singleBytes = BitConverter.GetBytes((float)_packedData[i]);
                Buffer.BlockCopy(singleBytes,0,resBytes,lastIdx,singleBytes.Length);
                lastIdx += 4;
                break;
            case TypeCode.Boolean:
                byte[] boolBytes = BitConverter.GetBytes((bool)_packedData[i]);
                Buffer.BlockCopy(boolBytes,0,resBytes,lastIdx,boolBytes.Length);
                lastIdx += boolBytes.Length;
                break;
           }
        }
        return resBytes;
    }
}