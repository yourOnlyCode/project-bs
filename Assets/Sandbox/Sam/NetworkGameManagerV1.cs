using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkGameManagerV1 : NetworkManager
{
    /// <summary>
    /// The default prefab to be used to create player objects on the server.
    /// <para>Player objects are created in the default handler for AddPlayer() on the server. Implementing OnServerAddPlayer overrides this behaviour.</para>
    /// </summary>
    [Header("Connected Player Object")]
    [FormerlySerializedAs("m_ConnectedPrefab")]
    [Tooltip("Prefab of the other player object. Prefab must have a Network Identity component. May be an empty game object or a full avatar.")]
    public GameObject otherPlayerPrefab;

    [SerializeField] private int minPlayers = 1;

    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayer> roomPlayers {get;} = new List<NetworkRoomPlayer>();

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
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene) {
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
        if (SceneManager.GetActiveScene().name == menuScene) {
            bool isLeader = roomPlayers.Count == 0;
            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer()
    {
        roomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState() {
        for(int i = 0; i < numPlayers-1; i++) {
            roomPlayers[i].HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart() {
        if (numPlayers < minPlayers) {return false;}

        for(int i = 0; i < numPlayers - 1; i++) {
            if (!roomPlayers[i].isReady) { return false;}
        }

        return true;

    }

    public void AddRoomPlayer(NetworkRoomPlayer player) {
        roomPlayers.Add(player);
    }
}
