using System.Text;

public class SynapseMessageFormatter
{
    public static byte[] EncodeMessage(NetMessage _netMessage){
        ConnectionManager.singleton.PrintWrap(string.Format("Creating byte array of length {0}", _netMessage.size + 2));
        byte[] msg = new byte[_netMessage.size + 2];
        msg[0] = (byte)_netMessage.size;
        msg[1] = (byte)_netMessage.opCode;
        for(short i = 0; i < _netMessage.size; i++){
            ConnectionManager.singleton.PrintWrap(string.Format("Populating idx {0}", i+2));
            msg[i+2] = _netMessage.val[i]; 
        }
        return msg;
    }

    public byte[] EncodeMessage(){
        return null;
    }
}
