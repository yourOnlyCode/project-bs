using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

[CreateAssetMenu (fileName ="NewMaterial", menuName = "Material Object")]
public class MaterialObject : ScriptableObject, IItem
{
    [SerializeField]
    private ITEMS _itemType;

    public ITEMS ItemType
    {
        get { return _itemType; }
        private set { _itemType = value; }
    }

    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private Animation _swingAnimation;


    public Sprite GetSprite()
    {
        return _sprite;
    }

    public Animation GetAnimation()
    {
        return _swingAnimation;
    }
}
