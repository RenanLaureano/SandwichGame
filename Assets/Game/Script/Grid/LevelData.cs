using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LevelCollection", menuName = "SandwichGame/Level/Level", order = 1)]
public class LevelData: ScriptableObject
{
    public int rows;
    public int columns;
    public int amountBreads;
    public int amountIngredients;
    public List<TileInfo> gridData;
}
