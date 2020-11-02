using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetails : MonoBehaviour
{
    [SerializeField] private string _characterName = null;
    [SerializeField] private string _characterDescription = null;


    private bool _selected = false;
    private bool _available = true;

    public string GetCharacterName()
    {
        return _characterName;
    }

    public string GetCharacterDescription()
    {
        return _characterDescription;
    }


    private void Update()
    {
        if (!_available)
        {
            this.GetComponent<Button>().enabled = false;
        }
    }

    public void SetAvailable(bool pAvailable)
    {
        _available = pAvailable;
    }
}
