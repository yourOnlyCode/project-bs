using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HarvestItems : NetworkBehaviour
{

    [SerializeField] private GameObject _harvestMaterial;
    [SerializeField] private float _launchDistance = 2.5f;
    [SerializeField] private int _hitsPerMat = 3;
    [SerializeField] private int _maxHits = 9;

    private int _hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    [Server]
    public void InstantiateMaterial(Vector3 treePosition) // todo: could add a hit increase as a parameter for better equipment.
    {
        if (_hitCount < _maxHits)
        {
            _hitCount++;
            if (_harvestMaterial && _hitCount % _hitsPerMat == 0)
            {
                Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f) * _launchDistance, Random.Range(-1f, 1f) * _launchDistance);
                GameObject material = Instantiate(_harvestMaterial, treePosition, _harvestMaterial.transform.rotation);
                NetworkServer.Spawn(material);
                material.GetComponent<Material>().SpawnItem(treePosition, spawnOffset);

            }
        } else
        {
            // Break Down Here.
        }

        
    }


}
