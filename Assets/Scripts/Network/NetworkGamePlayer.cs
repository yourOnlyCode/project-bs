using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkGamePlayer : NetworkBehaviour
{

    [SyncVar]
    private string _displayName = "Loading...";

    [SerializeField] private PlayerInformation _playerInfo = null;


    private NetworkGameManagerV1 room;

    private NetworkGameManagerV1 Room {
        get
        {

            if(room != null) { return room;}
            return room = NetworkManager.singleton as NetworkGameManagerV1;


        }
    }

    public override void OnStartClient()
    {
        Debug.Log("Start Client");
        DontDestroyOnLoad(gameObject);
        Room.AddGamePlayer(this);
    }

    public override void OnNetworkDestroy()
    {
        Room.gamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string pDisplayName)
    {
        this._displayName = pDisplayName;
    }

    [Server]
    public PlayerInformation GetPlayerInfo()
    {
        return _playerInfo;
    }
}
