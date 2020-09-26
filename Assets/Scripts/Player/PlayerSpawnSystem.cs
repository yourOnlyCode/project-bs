using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab = null;

    private static List<Transform> _spawnPoints = new List<Transform>();

    private int _nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        _spawnPoints.Add(transform);

        _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkGameManagerV1.OnServerReadied += SpawnPlayer;

    [ServerCallback]

    private void OnDestroy()
    {
        NetworkGameManagerV1.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextIndex);

        if(spawnPoint == null)
        {
            Debug.Log($"Missing spawn point for player {_nextIndex}");
            return;
        }

        GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[_nextIndex].position, _spawnPoints[_nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);

        _nextIndex++;
    }
}
