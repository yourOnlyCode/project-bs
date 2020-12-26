using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EquipableItems : NetworkBehaviour
{
    private static int _FORWARD = 0;
    private static int _BACKWARD = 1;
    private static int _RESET = 2;

    public static int AXE = 0;
    public static int PICKAXE = 1;
    public static int FISHING_ROD = 2;


    [SerializeField] private Transform _handlePosition;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private int _type = 0;
    [SerializeField] private bool _useRandom = false;
    [SerializeField] private float _randomProbability = 1f;

    private Collider2D _randomSpawner;
    private bool _animate = false;
    private float _rotateSpeed = 5f;
    private int _animationState = _FORWARD;
    private List<GameObject> itemsHit;

    [SyncVar]
    private Vector2 _serverPosition;

    private void Start()
    {
        itemsHit = new List<GameObject>();
    }

    public override void OnStartServer()
    {
        _serverPosition = transform.position;
    }

    [Server]
    public void Swing(Collider2D[] collisions)
    {
        for(int i = 0; i < collisions.Length; i++)
        {
            if(collisions[i].gameObject.layer == 11)
            {
                if (!_useRandom)
                {
                    collisions[i].gameObject.GetComponent<HarvestItems>().InstantiateMaterial(_type);
                } else
                {
                    _randomSpawner = collisions[i];
                }
            }
        }

        _animate = true;
        RpcSwing();
        Debug.Log("Animate!");
    }

    private void FixedUpdate()
    {
        if(GetComponent<NetworkIdentity>().isServer)
        {
            _serverPosition = transform.position;
        } else
        {
            transform.position = _serverPosition;
        }

        if (_animate)
        {
            //Debug.Log(transform.rotation);
          

            if(_animationState == _FORWARD)
            {
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), _rotateSpeed);
                if (Math.Abs(transform.rotation.z) >= .5)
                {
                    _animationState = _BACKWARD;
                    if(_useRandom)
                    {
                        _animate = false;
                    }
                }
            } else if(_animationState == _BACKWARD)
            {
                float initRot = transform.rotation.z;
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), -_rotateSpeed);
                if(transform.rotation.z * initRot <= 0)
                {
                    _animationState = _RESET;
                }
            } else if(_animationState == _RESET)
            {
                transform.rotation.Set(0f, 0f, 0f, 0f);
                _animate = false;
                _animationState = _FORWARD;
            }

            

        } else if(_useRandom && _animationState == _BACKWARD && GetComponent<NetworkIdentity>().isServer)
        {
            if(UnityEngine.Random.Range(0f, 1f) <= _randomProbability)
            {
                _randomSpawner.gameObject.GetComponent<HarvestItems>().InstantiateMaterial(_type);
                _animate = true;
                RpcSwing();
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.gameObject);
        if (!itemsHit.Contains(collision.gameObject))
        {
            itemsHit.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (itemsHit.Contains(collision.gameObject))
        {
            itemsHit.Remove(collision.gameObject);
        }
    }

    [ClientRpc]
    private void RpcSwing()
    {
        _animate = true;
    }

    [Server]
    public void ServerPickup()
    {
        gameObject.layer = 10;
        RpcPickup();
    }

    [ClientRpc]
    private void RpcPickup()
    {
        gameObject.layer = 10;
    }

    [Server]
    public void ServerDrop()
    {
        gameObject.layer = 9;
        RpcDrop();

    }

    [ClientRpc]
    private void RpcDrop()
    {
        gameObject.layer = 9;
    }


}
