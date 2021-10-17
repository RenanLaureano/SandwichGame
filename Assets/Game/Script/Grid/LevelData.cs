using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int rows;
    public int columns;
    public int amountBreads;
    public int amountIngredients;
    public List<TileInfo> gridData;
}
