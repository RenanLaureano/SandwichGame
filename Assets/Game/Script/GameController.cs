using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        GENERATING_GRID,
        PLAYING,
        ANIMING,
        GAMEOVER,
    }

    [SerializeField] private Button resetButton;
    [SerializeField] private Button undoButton;

    [Header("Varibles")]
    [SerializeField] private IntVariable gameStateVariable;

    [Header("Events")]
    [SerializeField] private IntEvent onChangeGameState;
    [SerializeField] private GameObjectEvent onMoveIngredient;
    [SerializeField] private VoidEvent onCancelCommand;

    private int level = 1;
    private int moves = 0;
    private GameState gameState;

    private LevelCollection levelCollection;
    private GridController gridController;
    private Stack<Command> undoCommands = new Stack<Command>();

    // Start is called before the first frame update
    void Start()
    {
        gridController = GetComponent<GridController>();

        levelCollection = Resources.LoadAll<LevelCollection>("Collections")[0];

        resetButton.onClick.AddListener(OnClickResetButton);
        undoButton.onClick.AddListener(OnClickUndoButton);

        if (levelCollection == null)
        {
            return;
        }

        gridController.CreateGrid(levelCollection.levels[level]);

        gameStateVariable.SetValue((int)GameState.PLAYING);

        //Register events
        onChangeGameState.Register(OnChangeState);
        onMoveIngredient.Register(Moved);
        onCancelCommand.Register(CancelLastCommand);
    }

    public void ExecuteNewCommand(Command command)
    {
        moves++;
        undoCommands.Push(command);
        command.Execute();
    }

    public void Moved(GameObject movedObject)
    {
        TileNodeObject tileNode = movedObject.GetComponent<TileNodeObject>();

        if (moves == (levelCollection.levels[level].amountBreads + levelCollection.levels[level].amountIngredients) - 1)
        {
            GaveOver(tileNode);
        }
    }

    private void OnChangeState(int state)
    {
        Debug.Log("state: " + state);
        gameState = (GameState)state;
    }
    private void GaveOver(TileNodeObject tileNode)
    {
        bool won = true;

        if (tileNode.TileInfo.tileType != TileInfo.TileType.BREAD)
        {
            won = false;
        }

        TileNodeObject lastChild = tileNode.GetLastChild();

        if (lastChild == null || lastChild.TileInfo.tileType != TileInfo.TileType.BREAD)
        {
            won = false;
        }

        Debug.Log("Win: " + won.ToString());
    }

    public void CancelLastCommand()
    {
        moves--;
        undoCommands.Pop();
    }

    private void OnClickResetButton()
    {
        if (undoCommands == null || undoCommands.Count <= 0 || gameState == GameState.ANIMING)
        {
            return;
        }

        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {

        while (undoCommands.Count > 0)
        {
            yield return new WaitWhile(() => gameState == GameState.ANIMING);
            undoCommands.Pop().Undo();
        }

        moves = 0;

        yield return null;
    }

    private void OnClickUndoButton()
    {
        if (undoCommands == null || undoCommands.Count <= 0 || gameState == GameState.ANIMING)
        {
            return;
        }

        moves--;
        Command command = undoCommands.Pop();
        command.Undo();
    }
}
