using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterSelection : NetworkBehaviour
{
    private int _selectedID = -1;
    [SerializeField] private GameObject[] _characterOptions = null;

    private void Start()
    {
        _characterOptions = Resources.LoadAll<GameObject>("CharacterOptions");


        for(int i = 0; i < _characterOptions.Length; i++)
        {
            GameObject option = Instantiate(_characterOptions[i], this.transform);
            
            option.transform.position = new Vector3(option.transform.position.x
                + (160 * i), option.transform.position.y);

            AddCharacterButtonListener(option.GetComponent<Button>(), i);

        }
        
    }


    public void SelectCharacter(int pIndex)
    {
        Debug.Log(pIndex);
    }

    private void AddCharacterButtonListener(Button pOption, int pIndex)
    {
        pOption.onClick.AddListener(() => SelectCharacter(pIndex));
    }

}
