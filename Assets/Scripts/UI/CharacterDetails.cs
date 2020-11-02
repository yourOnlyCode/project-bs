using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetails : MonoBehaviour
{
    [SerializeField] private string _characterName = null;
    [SerializeField] private string _characterDescription = null;
    [SerializeField] private Sprite _bigImage = null;


    private bool _selected = false;
    private bool _available = true;

    private Button _button = null;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public string GetCharacterName()
    {
        return _characterName;
    }

    public string GetCharacterDescription()
    {
        return _characterDescription;
    }

    public Sprite GetBigImage()
    {
        return _bigImage;
    }

    private void Update()
    {
        
    }

    public void SetAvailable(bool pAvailable)
    {
        _available = pAvailable;
        _button.enabled = _available;
    }

    public bool IsAvailable()
    {
        return _available;
    }

    public bool SetSelected(bool pSelected)
    {
        if(pSelected)
        {
            if(_selected)
            {
                SetAvailable(false);
                return false; // The value was not set as selected.
            } else
            {
                _selected = pSelected;
                SetAvailable(false);
                return true;
            }
            
        } else
        {
            _selected = pSelected;
            SetAvailable(true);

            return true;
        }
    }
}
