using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.U2D.Animation;

public class PlayerAnimationController : NetworkBehaviour
{
    public static int IDLE_ANIMATION = 0;
    public static int WALK_ANIMATION = 1;

    [SerializeField] private SpriteRenderer _hair = null;
    [SerializeField] private SpriteRenderer _weapon = null;
    [SerializeField] private GameObject _bones = null;
    [SerializeField] private Animator _animator = null;

    public void SetPlayerHair(Sprite pSprite)
    {
        _hair.sprite = pSprite;
    }

    public void Awake()
    {
        _weapon.sprite = null;
    }

    public void setAnimation(int pAnimation)
    {
        if (pAnimation == IDLE_ANIMATION) {
            _animator.SetBool("IsWalking", false);
        } else if (pAnimation == WALK_ANIMATION)
        {
            _animator.SetBool("IsWalking", true);
        }
    }
}
