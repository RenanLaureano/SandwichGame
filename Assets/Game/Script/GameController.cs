using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using TMPro;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        GENERATING_GRID,
        PLAYING,
        ANIMING,
        GAMEOVER,
    }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private Button saveGridButton;

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
        level = PlayerPrefs.GetInt("Level", 0);

        gridController = GetComponent<GridController>();

        levelCollection = Resources.LoadAll<LevelCollection>("Collections")[0];

        resetButton.onClick.AddListener(OnClickResetButton);
        undoButton.onClick.AddListener(OnClickUndoButton);
        saveGridButton.onClick.AddListener(OnClickSaveGridButton);

        if (levelCollection == null)
        {
            return;
        }

        if(level >= levelCollection.levels.Count)
        {
            level = 0;
        }

        textLevel.text = string.Format("Level {0}", level + 1);

        gridController.CreateGrid(levelCollection.levels[level]);

        gameStateVariable.SetValue((int)GameState.PLAYING);

        //Register events
        onChangeGameState.Register(OnChangeState);
        onMoveIngredient.Register(Moved);
        onCancelCommand.Register(CancelLastCommand);
    }

    private void OnClickSaveGridButton()
    {
        LevelData data = gridController.GetLevelData();

        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.rows = data.rows;
        levelData.columns = data.columns;
        levelData.amountBreads = data.amountBreads;
        levelData.amountIngredients = data.amountIngredients;
        levelData.gridData = data.gridData;

        string fileName = "Level-" + System.DateTime.Now.Ticks.ToString();

        // Save the container asset with the specified name
#if UNITY_EDITOR
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Collections/Levels/Saved/"+ fileName+".asset");
        AssetDatabase.CreateAsset(levelData, path);
#else
        string path = AssetDatabase.GenerateUniqueAssetPath(Application.persistentDataPath + "/Collections/Levels/Saved/"+ fileName+".asset");
        AssetDatabase.CreateAsset(levelCollection, path);
#endif
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

        if (won)
        {
            level++;

            if (level >= levelCollection.levels.Count)
            {
                level = 0;
            }

            PlayerPrefs.SetInt("Level", level);

            moves = 0;
            undoCommands.Clear();

            textLevel.text = string.Format("Level {0}", level + 1);

            gridController.DeleteGrid();
            gridController.CreateGrid(levelCollection.levels[level]);
        }
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
