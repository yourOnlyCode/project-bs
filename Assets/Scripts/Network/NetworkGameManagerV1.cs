using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkGameManagerV1 : NetworkManager
{
    [SerializeField] private int _minPlayers = 1;
    [SerializeField] private int _finalNumKillers = 1;
    private int _numKillers = 0;
    private int _numVillagers = 0;

    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Scene] [SerializeField] private string playScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer _roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer _gamePlayerPrefab = null;
    [SerializeField] private GameObject _playerSpawnSystem = null;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    [SerializeField] public List<NetworkRoomPlayer> roomPlayers {get;} = new List<NetworkRoomPlayer>();
    [SerializeField] public List<NetworkGamePlayer> gamePlayers { get; } = new List<NetworkGamePlayer>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        for(int i = 0; i < spawnablePrefabs.Length; i++) {
            ClientScene.RegisterPrefab(spawnablePrefabs[i]);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();

        NotifyPlayersOfReadyState();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
        NotifyPlayersOfReadyState();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != menuScene) {
            conn.Disconnect();
            return;
        }

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null) {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();
            roomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene) {
            bool isLeader = roomPlayers.Count == 0;
            NetworkRoomPlayer roomPlayerInstance = Instantiate(_roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer()
    {
        roomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState() {
        for(int i = 0; i < roomPlayers.Count; i++) {
            roomPlayers[i].HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart() {
        if (numPlayers < _minPlayers) {return false;}

        for(int i = 0; i < roomPlayers.Count; i++) {
            if (!roomPlayers[i].isReady) { return false;}
        }

        return true;

    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            _numVillagers = numPlayers - _numKillers;
            if(!IsReadyToStart()) { return; }

            ServerChangeScene(playScene);

        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            for (int i = roomPlayers.Count - 1; i >= 0; i-- )
            {
                var conn = roomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(_gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(roomPlayers[i].displayName);
                // Set Selected Sprite here.

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true); 
            }
        }

        base.ServerChangeScene(newSceneName);

    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName == playScene)
        {
            GameObject playerSpawnSystemInstance = Instantiate(_playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public void AddGamePlayer(NetworkGamePlayer pGamePlayer)
    {
        if (pGamePlayer.isServer) {
            int numLeft = numPlayers - (_numKillers + _numVillagers);

            if (_numKillers == _finalNumKillers)
            {
                pGamePlayer.GetPlayerInfo().SetPlayerRole(PlayerInformation.ROLE_VILLAGER);
                _numVillagers++;
            } else
            {
                float killChance = 1f / numLeft;
                if (UnityEngine.Random.Range(0f, 1f) > killChance)
                {
                    pGamePlayer.GetPlayerInfo().SetPlayerRole(PlayerInformation.ROLE_VILLAGER);
                    _numVillagers++;
                } else
                {
                    pGamePlayer.GetPlayerInfo().SetPlayerRole(PlayerInformation.ROLE_KILLER);
                    _numKillers++;
                }
            }
            Debug.Log(pGamePlayer.GetPlayerInfo().GetPlayerRole());
        }

        gamePlayers.Add(pGamePlayer);
    }

    public void RemoveGamePlayer(NetworkGamePlayer pGamePlayer)
    {
        if (pGamePlayer.GetPlayerInfo().GetPlayerRole() == PlayerInformation.ROLE_KILLER)
        {
            _numKillers--;
        } else
        {
            _numVillagers--;
        }

        gamePlayers.Remove(pGamePlayer);
    }

}
