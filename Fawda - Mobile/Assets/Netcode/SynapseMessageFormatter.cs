using System.Text;

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
}
