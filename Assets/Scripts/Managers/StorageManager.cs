using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Constants;
using System;

public class StorageManager : NetworkBehaviour
{

    public static int NUMBER_OF_INVENTORY_TYPES = 3;


    [SerializeField] private GameObject _storageContainerPrefab;

    private List<GameObject> _storageContainers;
    private SyncDictionary<ITEMS, int> _inventory = new SyncDictionary<ITEMS, int>();

    [Server]
    override public void OnStartServer()
    {
        // ORDER MATTERS
        _inventory.Add(ITEMS.Wood, 0);
        _inventory.Add(ITEMS.Stone, 0);
        _inventory.Add(ITEMS.Fish, 0);
        

        GetAllStorageContainers();
    }



    public override void OnStartClient()
    {
        base.OnStartClient();
        _inventory.Callback += InventoryChanged;
        GetAllStorageContainers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void CreateNewStorageContainer(Vector3 position)
    {
        GameObject storage = Instantiate(_storageContainerPrefab, position, new Quaternion());
        NetworkServer.Spawn(storage);
        storage.GetComponent<StorageArea>().SetStorageManager(this);
        _storageContainers.Add(storage);
        RpcRefreshStorageContainers();
    }

    [Server]
    public void AddInventory(ITEMS pType)
    {
        _inventory[pType] += 1;
    }

    [Server]
    public void RemoveInventory(ITEMS pType)
    {
        _inventory[pType] -= 1;
    }
    
    public SyncDictionary<ITEMS,int> GetInventory()
    {
        return _inventory;
    }

    private void GetAllStorageContainers()
    {
        _storageContainers = new List<GameObject>(GameObject.FindGameObjectsWithTag("storage"));
        if (_storageContainers == null)
        {
            _storageContainers = new List<GameObject>();
        }
        for (int i = 0; i < _storageContainers.Count; i++)
        {
            _storageContainers[i].GetComponent<StorageArea>().SetStorageManager(this);
        }
    }

    [ClientRpc]
    private void RpcRefreshStorageContainers()
    {
        GetAllStorageContainers();
    }


    private void InventoryChanged(SyncDictionary<ITEMS, int>.Operation op, ITEMS key, int item)
    {
        for (int i = 0; i < _storageContainers.Count; i++)
        {
            if (_storageContainers[i].GetComponent<StorageArea>())
            {
                _storageContainers[i].GetComponent<StorageArea>().DataChanged(_inventory);
            }
        }
    }


}
