using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StorageManager : NetworkBehaviour
{
    public static int NUMBER_OF_INVENTORY_TYPES = 3;
    public static int WOOD = 0;
    public static int STONE = 1;
    public static int FISH = 2;


    [SerializeField] private GameObject _storageContainerPrefab;

    private List<GameObject> _storageContainers;
    private SyncListInt _inventory = new SyncListInt();

    [Server]
    override public void OnStartServer()
    {
        for(int i = 0; i < NUMBER_OF_INVENTORY_TYPES; i++)
        {
            _inventory.Add(0);
        }

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
    public void AddInventory(int pType)
    {
        _inventory[pType] += 1;
    }

    [Server]
    public void RemoveInventory(int pType)
    {
        _inventory[pType] -= 1;
    }
    
    public SyncList<int> GetInventory()
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


    private void InventoryChanged(SyncList<int>.Operation op, int index, int pOld, int pNew)
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
