using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class CharacterSelection : NetworkBehaviour
{
    private int _selectedID = -1;
    [SerializeField] private GameObject[] _characterInitializedOptions = null;
    [SerializeField] private NetworkRoomPlayer _networkPlayer = null;
    [SerializeField] private GameObject[] _characterOptions = null;
    [SerializeField] private Color _selectedColor = new Color(200, 50, 50, 50);
    [SerializeField] private Color _notSelectedColor = new Color();
    [SerializeField] private Image _bigImage = null;
    [SerializeField] private TMP_Text _title = null;
    [SerializeField] private TMP_Text _description = null;
    

    private void Start()
    {
        _characterOptions = Resources.LoadAll<GameObject>("CharacterOptions");

        _characterInitializedOptions = new GameObject[_characterOptions.Length];

        for(int i = 0; i < _characterOptions.Length; i++)
        {
            GameObject option = Instantiate(_characterOptions[i], this.transform);
            
            option.transform.position = new Vector3(option.transform.position.x
                + (160 * i), option.transform.position.y);

            _characterInitializedOptions[i] = option;

            AddCharacterButtonListener(option.GetComponent<Button>(), i);

        }
        
    }


    public void SelectCharacter(int pIndex)
    {

        CharacterDetails character = _characterInitializedOptions[pIndex].GetComponent<CharacterDetails>();

        if (pIndex != _selectedID
            && character.IsAvailable())
        {
            if(character.SetSelected(true))
            {
                if (_selectedID != -1)
                { 
                    _characterInitializedOptions[_selectedID].GetComponent<CharacterDetails>().SetSelected(false);
                    _characterInitializedOptions[_selectedID].GetComponent<Image>().color = _notSelectedColor;
                }
                _selectedID = pIndex;
                _characterInitializedOptions[pIndex].GetComponent<Image>().color = _selectedColor;
                _bigImage.sprite = character.GetBigImage();
                _description.text = character.GetCharacterDescription();
                _title.text = character.GetCharacterName();
                _networkPlayer.CmdSetCharacterIndex(_selectedID, character.GetCharacterName());
            }
        }

    }

    private void AddCharacterButtonListener(Button pButton, int pIndex)
    {
        pButton.onClick.AddListener(() => SelectCharacter(pIndex));
    }

    public GameObject[] GetCharacterOptions()
    {
        return _characterInitializedOptions;
    }

    public int GetSelectedIndex()
    {
        return _selectedID;
    }

}
