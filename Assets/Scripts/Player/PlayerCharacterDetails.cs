using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerCharacterDetails : NetworkBehaviour
{
    private CharacterDetailObject _characterDetailObject;
    [SyncVar(hook = nameof(HandleCharacterNameChanged))]
    private string _characterName;

    public CharacterDetailObject GetCharacterDetails()
    {
        return _characterDetailObject;
    }

    [Server]
    public void SetCharacter(string pName)
    {
        _characterName = pName;
        LoadCharacter();
    }

    public void HandleCharacterNameChanged(string Old, string New) => SetClientCharacter();

    private void SetClientCharacter()
    {
        LoadCharacter();
    }

    private void LoadCharacter()
    {
        List<CharacterDetailObject> characterDetails = Resources.LoadAll<CharacterDetailObject>("CharacterDetails").ToList();
        for (int i = 0; i < characterDetails.Count; i++)
        {
            if (characterDetails[i].name == _characterName)
            {
                _characterDetailObject = characterDetails[i];
            }
        }

        if(_characterDetailObject)
        {
            SetSprites();
        } else
        {
            Debug.LogError("There was an issue setting the Character Detail Object. (PlayerCharacterDetails)");
        }
    }

    private void SetSprites()
    {
        GetComponent<PlayerAnimationController>().SetAnimations(_characterDetailObject.GetCharacterAnimations());
    }

}
