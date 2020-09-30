using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.U2D.Animation;

public class PlayerAnimationController : NetworkBehaviour
{
    [SerializeField] private Sprite _playerSprite = null;
    [SerializeField] private Animator _animator = null;

    public void SetPlayerSprite(Sprite pSprite)
    {
        _playerSprite = pSprite;
        GetComponentInChildren<SpriteRenderer>().sprite = _playerSprite;
        GetComponentInChildren<SpriteSkin>();
    }
}
