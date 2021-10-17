using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private const float START_POS_X = -2.0f;
    private const float START_POS_Y = 0f;
    private const float START_POS_Z = -8f;

    private const float TILE_SIZE_X = 1.25f;
    private const float TILE_SIZE_Z = 1.25f;

    private int rows;
    private int columns;
    private List<TileInfo> levelsGrid;
    private TileNodeObject[,] grid;

    //Tiles
    private Dictionary<TileInfo.TileType, IgredientData> igredients;

    void Start()
    {
        LoadObjectTiles();
    }

    public void CreateGrid(int rows, int columns, List<TileInfo> gridData)
    {
        this.rows = rows;
        this.columns = columns;
        levelsGrid = GenerateGridData(gridData);
        GenerateGrid();
    }

    private List<TileInfo> GenerateGridData(List<TileInfo> gridData)
    {
        List<TileInfo> grid = new List<TileInfo>();

        if(gridData !=null && gridData.Count > 0)
        {

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    grid.Add(CreateTileInfo(gridData,j,i));
                }
            }
        }
        
        return grid;
    }

    private TileInfo CreateTileInfo(List<TileInfo> gridData, int row, int column)
    {
        foreach (TileInfo tile in gridData)
        {
            if(tile.row == row && tile.column == column)
            return new TileInfo
            {
                row = tile.row,
                column = tile.column,
                tileType = tile.tileType
            };
        }

        return new TileInfo
        {
            row = row,
            column = column,
            tileType = TileInfo.TileType.EMPTY
        };
    }

    private void LoadObjectTiles()
    {
        IgredientData[] tiles = Resources.LoadAll<IgredientData>("Tiles/");
        igredients = new Dictionary<TileInfo.TileType, IgredientData>();
        foreach (IgredientData tile in tiles)
        {
            igredients.Add(tile.tileType, tile);
        }
    }

    private void GenerateGrid()
    {
        if(levelsGrid == null)
        {
            return;
        }

        grid = new TileNodeObject[rows, columns];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                TileInfo tileInfo = GetTileInfo(j, i);
                CreateTile(tileInfo, j, i);
            }
        }
    }

    private void CreateTile(TileInfo tileInfo, int j, int i)
    {
        GameObject tileObject = GetTitleObjectByType(tileInfo.tileType);
        Vector3 position = new Vector3(START_POS_X + TILE_SIZE_X * i, START_POS_Y, START_POS_Z + TILE_SIZE_Z * j);
        GameObject tempTile = Instantiate(tileObject, position, Quaternion.identity);

        //Setup tile node
        TileNodeObject tileNode = tempTile.AddComponent<TileNodeObject>();
        tileNode.isAvailable = tileInfo.tileType != TileInfo.TileType.EMPTY;
        tileNode.SetTileInfo(tileInfo);

        grid[j, i] = tileNode;
    }

    private TileInfo GetTileInfo(int row, int column)
    {
        foreach (TileInfo tile in levelsGrid)
        {
            if (tile.row == row && tile.column == column)
            {
                return tile;
            }
        }
        return null;
    }

    private GameObject GetTitleObjectByType(TileInfo.TileType type)
    {
        if (igredients.ContainsKey(type))
        {
            return igredients[type].prefab;
        }

        return null;
    }

    public TileNodeObject[,] GetGrid()
    {
        return grid;
    }

    public bool GridExist(int row, int column)
    {
        if(row < 0 || column < 0)
        {
            return false;
        }

        if(row > rows || column > columns)
        {
            return false;
        }

        return true;
    }

    public void DeleteGrid()
    {
        foreach(TileNodeObject tile in grid)
        {
            Destroy(tile.gameObject);
        }
    }
}
