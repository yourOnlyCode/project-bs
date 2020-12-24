using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EquipableItems : NetworkBehaviour
{
    private static int _FORWARD = 0;
    private static int _BACKWARD = 1;
    private static int _RESET = 2;


    [SerializeField] private Transform _handlePosition;
    [SerializeField] private BoxCollider2D _collider;

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

    public void Swing()
    {
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
            Debug.Log(transform.rotation);
          

            if(_animationState == _FORWARD)
            {
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), _rotateSpeed);
                if (Math.Abs(transform.rotation.z) >= .5)
                {
                    _animationState = _BACKWARD;
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

            

        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.gameObject);
        itemsHit.Add(collision.gameObject);
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


}
