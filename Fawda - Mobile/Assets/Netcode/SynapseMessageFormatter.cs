using System.Text;

public static class SynapseMessageFormatter
{
    public static byte[] EncodeMessage(NetMessage _netMessage){
        return new byte[2]{(byte)_netMessage.opCode, (byte) _netMessage.val};
    }
}
