using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class UIGenerator : MonoBehaviour
{

    [SerializeField] private GameObject _uiToSpawn;
    private GameObject _instance = null;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenUI()
    {
        if (!_instance)
        {
            _instance = Instantiate(_uiToSpawn);
        }
    }

    public void CloseUI()
    {
        if (_instance)
        {
            Destroy(_instance);
        }
    }

    public void PushData(SyncList<int> pData)
    {
        List<TMP_Text> textFields = _instance.GetComponent<UIDataFields>().GetTextFields();
        for(int i = 0; i < textFields.Count; i++)
        {
            textFields[i].SetText(pData[i].ToString());
        }
    }

    public bool IsUIOpen()
    {
        if(_instance)
        {
            return true;
        } else
        {
            return false;
        }
    }


}
