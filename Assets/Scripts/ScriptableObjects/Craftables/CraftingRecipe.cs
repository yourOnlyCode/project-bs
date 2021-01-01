using System.Collections.Generic;
using UnityEngine;

public struct MaterialAmount
{
    public UnityEngine.Material material;
    public int amount;
}

public struct EquipmentAmount
{
}

public class CraftingRecipe : ScriptableObject
{
    public List<MaterialAmount> Materials;
    public List<EquipmentAmount> Results;
}
