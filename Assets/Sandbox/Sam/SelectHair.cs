using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SelectHair : NetworkBehaviour
{
    [SerializeField] private Sprite[] _hairOptions = null;
    [SerializeField] private Image _hairImage = null;
    [SerializeField] private NetworkRoomPlayer _player = null;
    private int _hairSelected = 0;

    

    private void Start()
    {
        _hairImage.sprite = _hairOptions[_hairSelected];
    }

    public void LeftButton()
    {
        _hairSelected -= 1;
        if(_hairSelected < 0)
        {
            _hairSelected = _hairOptions.Length - 1;
        }

        ChangeHair();
    }

    public void RightButton()
    {
        _hairSelected += 1;
        if (_hairSelected > _hairOptions.Length - 1)
        {
            _hairSelected = 0;
        }

        ChangeHair();
    }

    private void ChangeHair()
    {
        _hairImage.sprite = _hairOptions[_hairSelected];
        // Set selected hair
    }

    public Sprite GetSelectedHair()
    {
        return _hairOptions[_hairSelected];
    }


}
