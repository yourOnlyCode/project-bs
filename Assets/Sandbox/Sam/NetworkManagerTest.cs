using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Mirror;

public class NetworkManagerTest : NetworkManager
{
    /// <summary>
    /// The default prefab to be used to create player objects on the server.
    /// <para>Player objects are created in the default handler for AddPlayer() on the server. Implementing OnServerAddPlayer overrides this behaviour.</para>
    /// </summary>
    [Header("Connected Player Object")]
    [FormerlySerializedAs("m_ConnectedPrefab")]
    [Tooltip("Prefab of the other player object. Prefab must have a Network Identity component. May be an empty game object or a full avatar.")]
    public GameObject otherPlayerPrefab;
}
