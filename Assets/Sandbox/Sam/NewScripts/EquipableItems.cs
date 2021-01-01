using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Constants;

public class EquipableItems : NetworkBehaviour
{
    public static int PICKED_UP_LAYER = 10;
    public static int NOT_PICKED_UP_LAYER = 9;

    protected static int _FORWARD = 0;
    protected static int _BACKWARD = 1;
    protected static int _RESET = 2;


    [SerializeField] protected Transform _handlePosition;
    [SerializeField] protected EquipableObject _equipableDetails;

    protected bool _animate = false;
    protected float _rotateSpeed = 5f;
    protected int _animationState = _FORWARD;

    [SyncVar]
    protected Vector2 _serverPosition;

    public override void OnStartServer()
    {
        _serverPosition = transform.position;
    }

    [Server]
    public virtual void Swing(Collider2D[] collisions)
    {
        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i].gameObject.layer == 11) // TODO: Get rid of magic number;
            {
                // TODO: Implement new item enum.
                SpawnMaterial(collisions[i].gameObject.GetComponent<HarvestItems>());
            }
        }

        _animate = true;
        RpcSwing();
    }
    protected virtual void Update()
    {
        if (!GetComponent<NetworkIdentity>().isServer)
        {
            Vector3 distance = new Vector3(_serverPosition.x, _serverPosition.y) - transform.position;
            if (distance.magnitude < .05f) // TODO: Get rid of magic number;
            {
                transform.position = _serverPosition;
            }
            else
            {
                transform.position += distance.normalized * 5f * Time.deltaTime; // TODO: Get rid of magic number.
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            _serverPosition = transform.position;
        }

            Animate();
    }

    protected virtual void Animate()
    {
        if (_animate)
        {
            if (_animationState == _FORWARD)
            {
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), _rotateSpeed);
                if (Math.Abs(transform.rotation.z) >= .5) // TODO: Magic number.
                {
                    _animationState = _BACKWARD;
                }
            }
            else if (_animationState == _BACKWARD)
            {
                float initRot = transform.rotation.z;
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), -_rotateSpeed);
                if (transform.rotation.z * initRot <= 0)
                {
                    _animationState = _RESET;
                }
            }
            else if (_animationState == _RESET)
            {
                transform.rotation.Set(0f, 0f, 0f, 0f);
                _animate = false;
                _animationState = _FORWARD;
            }
        }


    }

    [ClientRpc]
    protected virtual void RpcSwing()
    {
        _animate = true;
    }

    [Server]
    public virtual void ServerPickup()
    {
        gameObject.layer = PICKED_UP_LAYER;
        RpcPickup();
    }

    [ClientRpc]
    protected virtual void RpcPickup()
    {
        gameObject.layer = PICKED_UP_LAYER;
    }

    [Server]
    public virtual void ServerDrop()
    {
        gameObject.layer = NOT_PICKED_UP_LAYER;
        RpcDrop();

    }

    [ClientRpc]
    protected virtual void RpcDrop()
    {
        gameObject.layer = NOT_PICKED_UP_LAYER; ;
    }

    public ITEMS GetItemType()
    {
        return _equipableDetails.ItemType;
    }

    [Server]
    protected void SpawnMaterial(HarvestItems pHarvestItem)
    {
        pHarvestItem.InstantiateMaterial(_equipableDetails.ItemType);
    }


}
