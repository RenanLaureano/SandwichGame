using System;
using System.Collections.Generic;

[Serializable]
public class GridData
{
    public List<TileInfo> tiles;

    public GridData(GridData data)
    {
        tiles = new List<TileInfo>();

        foreach(TileInfo info in data.tiles)
        {
            tiles.Add(new TileInfo() { 
                row = info.row,
                column = info.column,
                tileType = info.tileType
            });
        }
    }
}
