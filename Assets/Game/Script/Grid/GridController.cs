using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private const float START_POS_X = -2.0f;
    private const float START_POS_Y = 0f;
    private const float START_POS_Z = -8f;

    private const float TILE_SIZE_X = 1.25f;
    private const float TILE_SIZE_Z = 1.25f;

    [Header("Varibles")]
    [SerializeField] private IntValueList igredientAvaliable;

    private int rows;
    private int columns;
    private int amountBreads;
    private int amountIngredients;
    private List<TileInfo> levelsGrid;
    private TileNodeObject[,] grid;

    //Tiles
    private Dictionary<TileInfo.TileType, IgredientData> igredients;

    void Start()
    {
        LoadObjectTiles();
    }

    public void CreateGrid(LevelData levelData)
    {
        amountBreads = levelData.amountBreads;
        amountIngredients = levelData.amountIngredients;
        rows = levelData.rows;
        columns = levelData.columns;
        levelsGrid = GenerateGridData(levelData.gridData);
        GenerateGrid();
    }

    private List<TileInfo> GenerateGridData(List<TileInfo> gridData)
    {
        List<TileInfo> grid = new List<TileInfo>();

        if (gridData == null || gridData.Count <= 0)
        {
            gridData = GenerateRandomGrid();
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                grid.Add(CreateTileInfo(gridData, j, i));
            }
        }

        return grid;
    }

    private List<TileInfo> GenerateRandomGrid()
    {
        List<TileInfo> grid = new List<TileInfo>();
        int iRow = Random.Range(0, rows);
        int iColumn = Random.Range(0, columns);
        int countIngredients = 0;
        int countBreads = 0;
        int countTiles = amountBreads + amountIngredients;
        bool nextBread = false;

        while (countBreads + countIngredients < countTiles)
        {
            int rRow = iRow;
            int rColumn = iColumn;

            if (grid.Count > 0)
            {
                int rIndex = !nextBread ? Random.Range(0, grid.Count) : grid.Count - 1;

                iRow = grid[rIndex].row;
                iColumn = grid[rIndex].column;

                if (Random.Range(0, 2) == 0)
                {
                    rRow = iRow + (Random.Range(0, 2) == 0 ? 1 : -1);
                    rColumn = iColumn;
                }
                else
                {
                    rColumn = iColumn + (Random.Range(0, 2) == 0 ? 1 : -1);
                    rRow = iRow;
                }
            }

            if (GridExist(rRow, rColumn) && GetTileInfo(grid, rRow, rColumn) == null)
            {
                int randomType = Random.Range(0, 2);
                TileInfo.TileType type = TileInfo.TileType.BREAD;

                if ((randomType == 0 && countIngredients < amountIngredients && !nextBread) || countBreads >= amountBreads)
                {
                    int rIgredient = Random.Range(0, igredientAvaliable.Count);
                    type = (TileInfo.TileType)igredientAvaliable.List[rIgredient];
                    countIngredients++;
                    nextBread = false;
                }
                else
                {
                    nextBread = false;

                    if (countBreads < 2)
                    {
                        nextBread = true;
                    }

                    countBreads++;
                }

                grid.Add(new TileInfo { row = rRow, column = rColumn, tileType = type });
            }
        }

        return grid;
    }


    private TileInfo CreateTileInfo(List<TileInfo> gridData, int row, int column)
    {
        foreach (TileInfo tile in gridData)
        {
            if (tile.row == row && tile.column == column)
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
        if (levelsGrid == null)
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
                return new TileInfo { row = row, column = column, tileType = tile.tileType };
            }
        }
        return null;
    }

    private TileInfo GetTileInfo(List<TileInfo> levelsGrid, int row, int column)
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
        if (row < 0 || column < 0)
        {
            return false;
        }

        if (row >= rows || column >= columns)
        {
            return false;
        }

        return true;
    }

    public void DeleteGrid()
    {
        foreach (TileNodeObject tile in grid)
        {
            Destroy(tile.gameObject);
        }
    }

    public LevelData GetLevelData()
    {
        return new LevelData { 
            rows = rows,
            columns = columns,
            amountBreads = amountBreads,
            amountIngredients = amountIngredients,
            gridData = levelsGrid
        };
    }
}
