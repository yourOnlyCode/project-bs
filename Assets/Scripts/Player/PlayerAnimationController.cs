using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.U2D.Animation;

public class PlayerAnimationController : NetworkBehaviour
{
    public static int IDLE_ANIMATION = 0;
    public static int WALK_ANIMATION = 1;

    [SerializeField] private Animator _animator = null;

    private AnimatorOverrideController _animatorOverrideController;

    public void Start()
    {
        InitializeOverrideAnimator();
    
    }

    public void SetAnimations(List<CharacterAnimation> pCharacterAnimations)
    {
        InitializeOverrideAnimator();
        if (_animatorOverrideController)
        {
            for (int i = 0; i < pCharacterAnimations.Count; i++)
            {
                _animatorOverrideController[pCharacterAnimations[i].animationType.ToString()] = pCharacterAnimations[i].animation;
             
            }

        } else
        {
            Debug.LogError("Override Controller not initialized");
        }
    }

    private void InitializeOverrideAnimator()
    {
        if(_animatorOverrideController)
        {
            return;
        }
        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animatorOverrideController;

    }

    public void setAnimation(int pAnimation, int pRight)
    {
        if (pAnimation == IDLE_ANIMATION) {
            _animator.SetBool("IsWalking", false);
        } else if (pAnimation == WALK_ANIMATION)
        {
            _animator.SetBool("IsWalking", true);
        }
        if (pRight >= 0)
        {
            _animator.SetBool("Right", pRight>0);
        }
        
    }
}
