using System.Collections.Generic;
using UnityEngine;
using static Constants;

public struct MaterialAmount
{
    public Material material;
    [Range(1,999)]
    public int amount;
}

public struct EquipmentAmount
{
    public IItem item; // TODO: May work on changing this to an ICraftable Type instead. 
    [Range(1,999)]
    public int amount;
}

public class CraftingRecipe : ScriptableObject
{
    public List<MaterialAmount> Materials;
    public List<EquipmentAmount> Results;

    public bool CanCraft(Dictionary<ITEMS, int> pInventory)
    {
        bool canCraft = true;
        for (int i = 0; i < Materials.Count; i++)
        {
            if(pInventory[Materials[i].material.GetMaterialType()] < Materials[i].amount)
            {
                canCraft = false;
            }
        }

        return canCraft;
    }
}
