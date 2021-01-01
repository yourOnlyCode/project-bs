using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EquipableFishingRod : EquipableItems
{

    [SerializeField] private float _randomProbability = 1f;


    private Collider2D _randomSpawner;

    [Server]
    public override void Swing(Collider2D[] collisions)
    {
        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i].gameObject.layer == 11)
            {
                    _randomSpawner = collisions[i];
            }
        }
        
        _animate = true;
        RpcSwing();
    }


    protected override void Animate()
    {
        if (_animate)
        {

            if (_animationState == _FORWARD)
            {
                transform.RotateAround(_handlePosition.position, new Vector3(0f, 0f, 1f), _rotateSpeed);
                if (Math.Abs(transform.rotation.z) >= .5)
                {
                    _animationState = _BACKWARD;
                    if (_randomSpawner)
                    {
                        _animate = false;
                    }

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
        else if (_animationState == _BACKWARD && GetComponent<NetworkIdentity>().isServer)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= _randomProbability)
            {

                SpawnMaterial(_randomSpawner.gameObject.GetComponent<HarvestItems>());
                _animate = true;
                RpcSwing();
            }
        }
    }

}
