using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Material : NetworkBehaviour
{
    [SerializeField] private float _bounceHeight = 3f;

    private bool _spawnAnimation = false;
    private Vector3 _initialPos;
    private Vector3 _spawnPosition;
    private Vector3 _spawnOffset;
    private Vector3 _animationProgress;

    [SyncVar]
    private Vector3 _serverPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_spawnAnimation)
        {
                {
                    _animationProgress += _spawnOffset / 30;

                    if(_animationProgress.magnitude < _spawnOffset.magnitude/2)
                    {
                        transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight/Mathf.Pow(_spawnOffset.magnitude / 4, 2) * - Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude / 4, 2) + _bounceHeight);
                    } else if (_animationProgress.magnitude > _spawnOffset.magnitude / 2 && _animationProgress.magnitude < _spawnOffset.magnitude / 4 * 3)
                    {
                        transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight * .5f / Mathf.Pow(_spawnOffset.magnitude / 8 * 5, 2) * -Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude /  8 * 5, 2) + _bounceHeight * .5f);
                    } else if (_animationProgress.magnitude > _spawnOffset.magnitude / 4 * 3 && _animationProgress.magnitude < _spawnOffset.magnitude)
                    {
                        transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight * .5f / Mathf.Pow(_spawnOffset.magnitude / 8, 2) * - Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude / 8 * 7, 2) + _bounceHeight * .5f);
                    } else if(_animationProgress.magnitude > _spawnOffset.magnitude)
                    {
                        transform.position = _spawnPosition;
                        _spawnAnimation = false;
                    }
                }
        }

        if(GetComponent<NetworkIdentity>().isServer)
        {
            _serverPosition = transform.position;
        } else
        {
            if (!_spawnAnimation)
            {
                transform.position = _serverPosition;
            }
        }
    }

    [Server]
    public void SpawnItem(Vector3 pLocation, Vector3 pOffsetLocation)
    {
        _initialPos = pLocation;
        _spawnPosition = pLocation + pOffsetLocation;
        _spawnOffset = pOffsetLocation;
        _animationProgress = new Vector3();
        _spawnAnimation = true;

        RpcSpawn(pLocation, pOffsetLocation);
    }

    [ClientRpc]
    private void RpcSpawn(Vector3 pLocation, Vector3 pOffsetLocation)
    {
        _initialPos = pLocation;
        _spawnPosition = pLocation + pOffsetLocation;
        _spawnOffset = pOffsetLocation;
        _animationProgress = new Vector3();
        _spawnAnimation = true;
    }

    [Server]
    public void ServerPickup()
    {
        gameObject.layer = 13;
        RpcPickup();
    }

    [ClientRpc]
    private void RpcPickup()
    {
        gameObject.layer = 13;
    }

    [Server]
    public void ServerDrop()
    {
        gameObject.layer = 12;
        RpcDrop();

    }

    [ClientRpc]
    private void RpcDrop()
    {
        gameObject.layer = 12;
    }
}
