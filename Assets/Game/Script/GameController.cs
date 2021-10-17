using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    [SerializeField] private Button undoButton;
    private int level = 0;
    private int moves = 0;

    private LevelCollection[] levelCollections;
    private GridController gridController;
    private Stack<Command> undoCommands = new Stack<Command>();

    // Start is called before the first frame update
    void Start()
    {
        gridController = GetComponent<GridController>();

        levelCollections = Resources.LoadAll<LevelCollection>("Collections/");

        resetButton.onClick.AddListener(OnClickResetButton);
        undoButton.onClick.AddListener(OnClickUndoButton);

        if (levelCollections.Length <= 0 || levelCollections[level].gridData.Count <= 0)
        {
            return;
        }

        LevelCollection _level = levelCollections[level];
        gridController.CreateGrid(_level.rows, _level.columns, _level.gridData);
    }

    public void ExecuteNewCommand(Command command)
    {
        moves++;
        undoCommands.Push(command);
        command.Execute();
    }

    public void Moved(TileNodeObject tileNode)
    {
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

    public void CancelLastCommand()
    {
        undoCommands.Pop();
    }

    private void OnClickResetButton()
    {
        MoveController moveController = ServiceLocator.Instance.GetComponentRegistered<MoveController>();

        if (undoCommands == null || undoCommands.Count <= 0 || moveController.InMoving)
        {
            return;
        }

        StartCoroutine(RestartGame(moveController));
    }

    private IEnumerator RestartGame(MoveController moveController)
    {

        while (undoCommands.Count > 0)
        {
            yield return new WaitWhile(() => moveController.InMoving);
            undoCommands.Pop().Undo();
        }

        yield return null;
    }

    private void OnClickUndoButton()
    {
        MoveController moveController = ServiceLocator.Instance.GetComponentRegistered<MoveController>();

        if (undoCommands == null || undoCommands.Count <= 0 || moveController.InMoving)
        {
            return;
        }

        moves--;
        Command command = undoCommands.Pop();
        command.Undo();
    }
}
