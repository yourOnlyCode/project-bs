using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Constants;

public class Material : NetworkBehaviour
{
    [SerializeField] protected float _bounceHeight = 3f;
    [SerializeField] protected MaterialObject _materialDetails;

    protected bool _spawnAnimation = false;
    protected Vector3 _initialPos;
    protected Vector3 _spawnPosition;
    protected Vector3 _spawnOffset;
    protected Vector3 _animationProgress;

    protected float _movementSpeed = 5f;

    protected GameObject _storageContainer;

    [SyncVar]
    protected Vector3 _serverPosition;

    protected void Update()
    {
        if(!GetComponent<NetworkIdentity>().isServer)
        {
            if (!_spawnAnimation)
            {
                Vector3 distance = _serverPosition - transform.position;
                if (distance.magnitude < .05f)
                {
                    transform.position = _serverPosition;
                }
                else
                {
                    transform.position += distance.normalized * 5f * Time.deltaTime;
                }
            }
        }
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (_spawnAnimation)
        {
            SpawnAnimation();
        }

        if(GetComponent<NetworkIdentity>().isServer)
        {
            _serverPosition = transform.position;
        }
    }

    protected virtual void SpawnAnimation()
    {
        _animationProgress += _spawnOffset / 30;

        if (_animationProgress.magnitude < _spawnOffset.magnitude / 2)
        {
            transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight / Mathf.Pow(_spawnOffset.magnitude / 4, 2) * -Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude / 4, 2) + _bounceHeight);
        }
        else if (_animationProgress.magnitude > _spawnOffset.magnitude / 2 && _animationProgress.magnitude < _spawnOffset.magnitude / 4 * 3)
        {
            transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight * .5f / Mathf.Pow(_spawnOffset.magnitude / 8 * 5, 2) * -Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude / 8 * 5, 2) + _bounceHeight * .5f);
        }
        else if (_animationProgress.magnitude > _spawnOffset.magnitude / 4 * 3 && _animationProgress.magnitude < _spawnOffset.magnitude)
        {
            transform.position = _initialPos + _animationProgress + new Vector3(0f, _bounceHeight * .5f / Mathf.Pow(_spawnOffset.magnitude / 8, 2) * -Mathf.Pow(_animationProgress.magnitude - _spawnOffset.magnitude / 8 * 7, 2) + _bounceHeight * .5f);
        }
        else if (_animationProgress.magnitude > _spawnOffset.magnitude)
        {
            transform.position = _spawnPosition;
            _spawnAnimation = false;
        }
    }

    [Server]
    public void SpawnItem(Vector3 pLocation, Vector3 pOffsetLocation)
    {
        Debug.Log("Spawn Item");
        _initialPos = pLocation;
        _spawnPosition = pLocation + pOffsetLocation;
        _spawnOffset = pOffsetLocation;
        _animationProgress = new Vector3();
        _spawnAnimation = true;

        RpcSpawn(pLocation, pOffsetLocation);
    }

    [ClientRpc]
    protected void RpcSpawn(Vector3 pLocation, Vector3 pOffsetLocation)
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

        GetComponent<BoxCollider2D>().enabled = false;
        gameObject.layer = 13;
        RpcPickup();
    }

    [ClientRpc]
    protected void RpcPickup()
    {

        GetComponent<BoxCollider2D>().enabled = false;
        gameObject.layer = 13;

    }

    [Server]
    public void ServerDrop()
    {
        gameObject.layer = 12;
        GetComponent<BoxCollider2D>().enabled = true;
        RpcDrop();

    }

    [ClientRpc]
    protected void RpcDrop()
    {
        gameObject.layer = 12;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public ITEMS GetMaterialType()
    {
        return _materialDetails.ItemType;
    }
}
