public enum OpCode{
    QUIT = -1,
    INDEX = 0,
    PROFILE_PAYLOAD = 1,
    MENU_CONTROL = 2,
    UDP_INPUT = 3
}

public struct NetMessage{
    public byte size;
    public OpCode opCode;
    public byte[] val;
    public NetMessage(OpCode _opcode, byte[] _val){
        this.opCode = _opcode;
        this.val = _val;
        this.size = (byte)_val.Length;
    }
}

public struct DirectedNetMessage{
    public NetMessage msg;
    public short client;
    public DirectedNetMessage(NetMessage _msg, short _client){
        this.msg = _msg;
        this.client = _client;
    }
}