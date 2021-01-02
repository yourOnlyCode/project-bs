using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Constants;

public class HarvestItems : NetworkBehaviour
{
    [SerializeField] private GameObject _harvestMaterial;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Sprite _deadSprite;
    [SerializeField] private float _launchDistance = 2.5f;
    [SerializeField] private int _hitsPerMat = 3;
    [SerializeField] private int _maxHits = 9;
    [SerializeField] private ITEMS _equipmentType;

    private int _hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    [Server]
    public void InstantiateMaterial(ITEMS pEquipmentType) // todo: could add a hit increase as a parameter for better equipment.
    {
        if (_equipmentType == pEquipmentType)
        {
            if (_hitCount < _maxHits || _maxHits == 0)
            {
                _hitCount++;
                if (_harvestMaterial && _hitCount % _hitsPerMat == 0)
                {
                    Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f) * _launchDistance, Random.Range(-1f, 1f) * _launchDistance);
                    GameObject material = Instantiate(_harvestMaterial, _spawnPosition.position, _harvestMaterial.transform.rotation);
                    NetworkServer.Spawn(material);
                    material.GetComponent<Material>().SpawnItem(_spawnPosition.position, spawnOffset);

                }
                if(_hitCount == _maxHits)
                {
                    if(_deadSprite == null)
                    {
                        GetComponent<BoxCollider2D>().enabled = false;
                    }
                    GetComponent<SpriteRenderer>().sprite = _deadSprite;
                    RpcSetDeadSprite();
                }
            }
        }

        
    }

    [ClientRpc]
    private void RpcSetDeadSprite()
    {
        if (_deadSprite == null)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        GetComponent<SpriteRenderer>().sprite = _deadSprite;
    }


}
