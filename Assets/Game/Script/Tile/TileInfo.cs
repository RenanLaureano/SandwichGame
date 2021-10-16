using System;

[Serializable]
public class TileInfo
{
    public enum TileType
    {
        EMPTY,
        BREAD,
        BACON,
        CHEESE,
        EGG,
        HAM,
        ONION,
        SALAD,
        SALAMI,
        TOMATO
    }

    public int row;
    public int column;
    public TileType tileType;
}
