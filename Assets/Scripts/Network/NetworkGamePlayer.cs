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

    [SerializeField] private GameObject _playerGameObject = null;


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
        Debug.Log("Finish Start Client");
    }

    public override void OnNetworkDestroy()
    {
        Room.gamePlayers.Remove(this);
    }

    public void Update()
    {

    }

    [Server]
    public void SetDisplayName(string pDisplayName)
    {
        this._displayName = pDisplayName;
    }

    [Server]
    public PlayerInformation GetPlayerInfo()
    {
        if(_playerInfo == null)
        {
            _playerInfo = gameObject.AddComponent<PlayerInformation>();
        }

        return _playerInfo;
    }

    [Server]
    public void SetPlayerGameObject(GameObject pPlayer)
    {
        pPlayer.GetComponent<PlayerGameObject>().SetOwner(this);
        _playerGameObject = pPlayer;
    }

    public GameObject getPlayerGameObject()
    {
        return _playerGameObject;
    }



}
