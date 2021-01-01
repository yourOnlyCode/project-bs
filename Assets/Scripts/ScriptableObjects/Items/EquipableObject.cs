using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "NewEquipable", menuName = "Equipable")]
public class EquipableObject : ScriptableObject, IItem
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
    private int _damage;
    [SerializeField]
    private Animation _swingAnimation;


    public Sprite GetSprite()
    {
        return _sprite;
    }

    public int GetDamage()
    {
        return _damage;
    }

    public Animation GetAnimation()
    {
        return _swingAnimation;
    }
}
