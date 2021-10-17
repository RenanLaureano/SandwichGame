using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Igredient", menuName = "SandwichGame/Igredient", order = 2)]

public class IgredientData : ScriptableObject
{
    public TileInfo.TileType tileType;
    public GameObject prefab;
}
