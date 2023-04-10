public enum OpCode{
    PROFILE_PAYLOAD = 0,
    MENU_CONTROL = 1,
    UDP_ENGAGE = 2
}

public struct NetMessage{
    public OpCode opCode;
    public byte val;
    public NetMessage(OpCode _opcode, byte _val){
        this.opCode = _opcode;
        this.val = _val;
    }
}