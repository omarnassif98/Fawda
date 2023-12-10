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
         DebugLogger.singleton.Log("Packing");

        for(int i=0; i<_packedData.Count; i++){
           
           switch (Type.GetTypeCode(_packedData[i].GetType()))
           {
            case TypeCode.String:
                DebugLogger.singleton.Log("STR");
            
                length += 12;
                break;
                    //Strings are to be 12 letters or less
            case TypeCode.Int32:
            case TypeCode.Int16:
                DebugLogger.singleton.Log("INT");
                length += 1;
                break;
            case TypeCode.Single:
                DebugLogger.singleton.Log("FLOAT");
                length += 4;
            break;
           }
        }

        byte[] resBytes = new byte[length];
        DebugLogger.singleton.Log(string.Format("Byte array of length {0}",length));
        for(int i=0; i<_packedData.Count; i++){
           DebugLogger.singleton.Log(string.Format("IDX: {0}", lastIdx));
           switch (Type.GetTypeCode(_packedData[i].GetType()))
           {
            case TypeCode.String:
                string fixedString = ((string)_packedData[i]).PadRight(12).Substring(0,12);
                ClientConnection.singleton.PrintWrap(fixedString);
                byte[] stringBytes = System.Text.Encoding.ASCII.GetBytes(fixedString);
                Buffer.BlockCopy(stringBytes,0,resBytes,lastIdx,stringBytes.Length);
                lastIdx += stringBytes.Length;
                DebugLogger.singleton.Log(String.Format("{0} byte string", stringBytes.Length.ToString()));
                break;
                    //Strings are to be 9 letters or less
            case TypeCode.Int32:
                DebugLogger.singleton.Log(string.Format("Attempting int: {0} ({1})",_packedData[i], Mathf.Clamp((int)_packedData[i],0,255)));
                byte val = (byte)Mathf.Clamp((int)_packedData[i],0,255);
                resBytes[lastIdx] = val;
                lastIdx += 1;
                DebugLogger.singleton.Log("1 byte int (0-255)");
                break;
            case TypeCode.Single:
                byte[] singleBytes = BitConverter.GetBytes((float)_packedData[i]);
                DebugLogger.singleton.Log(string.Format("Packed {0} as {1}", ((float)_packedData[i]).ToString(), BitConverter.ToString(singleBytes)));
                Buffer.BlockCopy(singleBytes,0,resBytes,lastIdx,singleBytes.Length);
                lastIdx += 4;
            break;
           }
        }
        return resBytes;
    }
}