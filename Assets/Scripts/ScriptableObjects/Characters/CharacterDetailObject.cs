using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

[Serializable]
public struct CharacterAnimation
{
    [SerializeField]
    public CHARACTER_ANIMATIONS animationType;
    [SerializeField]
    public AnimationClip animation;
}

[CreateAssetMenu(fileName = "NewCharacterDetails", menuName = "Character")]
public class CharacterDetailObject : ScriptableObject
{
    [SerializeField] private List<CharacterAnimation> characterAnimations;

    public List<CharacterAnimation> GetCharacterAnimations()
    {
        return characterAnimations;
    }
}
