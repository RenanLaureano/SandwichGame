using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LevelCollection", menuName = "SandwichGame/LevelCollection", order = 1)]
public class LevelCollection : ScriptableObject
{
    public int rows;
    public int columns;
    public int levels;
    public int amountBreads;
    public int amountIngredients;
    public List<TileInfo> gridData;
}
