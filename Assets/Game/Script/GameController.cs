using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int level = 0;
    private int moves = 0;

    private LevelCollection[] levelCollections;
    private GridController gridController;
    // Start is called before the first frame update
    void Start()
    {
        gridController = GetComponent<GridController>();

        levelCollections = Resources.LoadAll<LevelCollection>("Collections/");

        if (levelCollections.Length <= 0 || levelCollections[level].gridData.Count <= 0)
        {
            return;
        }

        LevelCollection _level = levelCollections[level];
        gridController.CreateGrid(_level.rows, _level.columns, _level.gridData);
    }

    public void Moved(TileNodeObject tileNode)
    {
        moves++;

        if (moves == (levelCollections[0].amountBreads + levelCollections[0].amountIngredients) - 1)
        {
            if (CheckWin(tileNode))
            {
                Debug.Log("WIN");
            }
            else
            {
                Debug.Log("LOSE");
            }
        }
    }

    private bool CheckWin(TileNodeObject tileNode)
    {
        if (tileNode.TileInfo.tileType != TileInfo.TileType.BREAD)
        {
            return false;
        }

        TileNodeObject lastChild = tileNode.GetLastChild();

        if (lastChild == null || lastChild.TileInfo.tileType != TileInfo.TileType.BREAD)
        {
            return false;
        }

        return true;
    }
}
