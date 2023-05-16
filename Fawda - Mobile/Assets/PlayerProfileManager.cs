using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfileManager : MonoBehaviour
{
    private short idx;
    public static PlayerProfileManager singleton;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetPlayerConnectionIndex(byte[] _data)
    {
        idx = (short) _data[0];
    }

    public short GetPlayerConnectionIndex()
    {
        return idx;
    }


    // Start is called before the first frame update
    void Start()
    {
        ClientConnection.singleton.RegisterRPC("INDEX", SetPlayerConnectionIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
