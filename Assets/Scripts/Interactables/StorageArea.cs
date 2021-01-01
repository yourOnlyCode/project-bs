using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Constants;

public class StorageArea : NetworkBehaviour
{

    private StorageManager _storageManager;
    private Dictionary<ITEMS, int> _inventory = new Dictionary<ITEMS, int>() {
        
        {ITEMS.Wood , 0 },
        { ITEMS.Stone , 0 },
        {ITEMS.Fish, 0 }
    };

    private UIGenerator ui;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        if(!_storageManager)
        {
            _storageManager = FindObjectOfType<StorageManager>();
        }
        ui = GetComponent<UIGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer)
        {
            if (collision.gameObject.layer == 12)
            {
                ITEMS type = collision.gameObject.GetComponent<Material>().GetMaterialType();
                _inventory[type] += 1;
                _storageManager.AddInventory(type);
            }
        }


        if (collision.GetComponent<NetworkIdentity>().hasAuthority)
        {
            if(collision.CompareTag("Player"))
            {
                ui.OpenUI();
                ui.PushData(_storageManager.GetInventory());
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isServer)
        {
            if (collision.gameObject.layer == 12)
            {
                ITEMS type = collision.gameObject.GetComponent<Material>().GetMaterialType();
                _inventory[type] -= 1;
                _storageManager.RemoveInventory(type);
            }
        }
        if (collision.GetComponent<NetworkIdentity>().hasAuthority)
        {
            if (collision.CompareTag("Player"))
            {
                ui.CloseUI();
            }
        }
    }

    [Server]
    public void SetStorageManager(StorageManager pManager)
    {
        _storageManager = pManager;
    }

    [Server]
    public Dictionary<ITEMS,int> GetInventory()
    {
        return _inventory;
    }

    public void DataChanged(SyncDictionary<ITEMS, int> pData)
    {
        if(ui.IsUIOpen())
        {
            ui.PushData(pData);
        }
    }



}
