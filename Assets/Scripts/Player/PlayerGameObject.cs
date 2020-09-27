using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerGameObject : NetworkBehaviour
{
    private NetworkGamePlayer _owner = null;

    public void SetOwner(NetworkGamePlayer pOwner)
    {
        _owner = pOwner;
    }

    public NetworkGamePlayer GetOwner()
    {
        return _owner;
    }

    public PlayerInformation GetPlayerInformation()
    {
        return _owner.GetPlayerInfo();
    }
}
