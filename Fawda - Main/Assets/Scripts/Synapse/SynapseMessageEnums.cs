public enum OpCode{
    QUIT = -1,
    INDEX = 0,
    PROFILE_PAYLOAD = 1,
    MENU_OCCUPY = 2,
    MENU_OCCUPATION_STATUS = 3,
    MENU_CONTROL = 4,
    UDP_TOGGLE = 5,
    UDP_GAMEPAD_INPUT = 6,
    INPUT_SETUP = 7,
    GAMESETUP = 8,
    READYUP = 9
}
public enum GameCodes{
    HAUNT = 0
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
    public int client;
    public DirectedNetMessage(NetMessage _msg, int _client){
        this.msg = _msg;
        this.client = _client;
    }
}