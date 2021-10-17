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
    private GameObject bread;
    private GameObject bacon;
    private GameObject cheese;
    private GameObject egg;
    private GameObject ham;
    private GameObject onion;
    private GameObject salad;
    private GameObject salami;
    private GameObject tomato;
    private GameObject empty;

    void Start()
    {
        LoadObjectTiles();
        GenerateGrid();
    }

    public void CreateGrid(int rows, int columns, List<TileInfo> gridData)
    {
        this.rows = rows;
        this.columns = columns;
        levelsGrid = GenerateGridDate(gridData);
    }

    private List<TileInfo> GenerateGridDate(List<TileInfo> gridData)
    {
        List<TileInfo> grid = new List<TileInfo>();

        if(gridData !=null && gridData.Count > 0)
        {
            foreach(TileInfo tile in gridData)
            {
                grid.Add(new TileInfo
                    { 
                    row = tile.row,
                    column = tile.column,
                    tileType = tile.tileType
                });
            }
        }

        return grid;
    }

    private void LoadObjectTiles()
    {
        bread = Resources.Load<GameObject>("Tiles/Bread");
        bacon = Resources.Load<GameObject>("Tiles/Bacon");
        cheese = Resources.Load<GameObject>("Tiles/Cheese");
        egg = Resources.Load<GameObject>("Tiles/Egg");
        ham = Resources.Load<GameObject>("Tiles/Ham");
        onion = Resources.Load<GameObject>("Tiles/Onion");
        salad = Resources.Load<GameObject>("Tiles/Salad");
        salami = Resources.Load<GameObject>("Tiles/Salami");
        tomato = Resources.Load<GameObject>("Tiles/Tomato");
        empty = Resources.Load<GameObject>("Tiles/Empty");
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
        switch (type)
        {
            case TileInfo.TileType.EMPTY:
                return empty;
            case TileInfo.TileType.BREAD:
                return bread;
            case TileInfo.TileType.BACON:
                return bacon;
            case TileInfo.TileType.CHEESE:
                return cheese;
            case TileInfo.TileType.EGG:
                return egg;
            case TileInfo.TileType.HAM:
                return ham;
            case TileInfo.TileType.ONION:
                return onion;
            case TileInfo.TileType.SALAD:
                return salad;
            case TileInfo.TileType.SALAMI:
                return salami;
            case TileInfo.TileType.TOMATO:
                return tomato;
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
