using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button buttonPause;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonOpenSavedGrids;
    [SerializeField] private float timeAnimation = 0.3f;
    [SerializeField] private GameObject content;
    [SerializeField] private CanvasGroup groupCanvas;

    [Header("Main screen")]
    [SerializeField] private GameObject mainScreen;
    [Header("Grids screen")]
    [SerializeField] private Button buttonBackScreen;
    [SerializeField] private GameObject gridsScreen;
    [SerializeField] private ButtonSavedGrid prefabButtonSavedGrids;
    [SerializeField] private Transform contentButtons;

    bool isPaused;

    int idAnimation;
    // Start is called before the first frame update
    void Start()
    {
        buttonPause.onClick.AddListener(OnClickPause);
        buttonPlay.onClick.AddListener(OnClickPause);
        buttonBackScreen.onClick.AddListener(BackMainScreen);
        buttonOpenSavedGrids.onClick.AddListener(OnClickOpenSavedGrids);
    }

    private void OnClickOpenSavedGrids()
    {
        mainScreen.SetActive(false);
        gridsScreen.SetActive(true);

        for (int i = 1; i < contentButtons.childCount; i++)
        {
            Destroy(contentButtons.GetChild(i).gameObject);
        }

#if UNITY_EDITOR
        LevelData[] savedGrids = Resources.LoadAll<LevelData>("Collections/Levels/Saved/");
#else
        List<LevelData> levelDatas = new List<LevelData>();
        string path = System.IO.Path.Combine(Application.persistentDataPath);
        int num = 0;
        foreach (var file in System.IO.Directory.EnumerateFiles(path, "*.json"))
        {
            string loadedJsonDataString = System.IO.File.ReadAllText(file);
            LevelDataJson data = JsonUtility.FromJson<LevelDataJson> (loadedJsonDataString);

            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            levelData.name = "Level-" + num.ToString();
            levelData.rows = data.rows;
            levelData.columns = data.columns;
            levelData.amountBreads = data.amountBreads;
            levelData.amountIngredients = data.amountIngredients;
            levelData.gridData = data.gridData;

            levelDatas.Add(levelData);
            num++;
        }
        LevelData[] savedGrids = levelDatas.ToArray();
#endif

        foreach (LevelData levelData in savedGrids)
        {
            Instantiate(prefabButtonSavedGrids, contentButtons).Init(levelData, OnPlaySavedLevel);
        }
    }

    private void OnPlaySavedLevel()
    {
        BackMainScreen();
        OnClickPause();
    }

    private void BackMainScreen()
    {
        mainScreen.SetActive(true);
        gridsScreen.SetActive(false);
    }
    private void OnClickPause()
    {
        isPaused = !isPaused;

        LeanTween.cancel(idAnimation);

        if (!isPaused)
        {
            idAnimation = LeanTween.alphaCanvas(groupCanvas, 0, timeAnimation)
                .setOnComplete(() => content.SetActive(false))
                .uniqueId;
        }
        else
        {
            content.SetActive(true);
            idAnimation = LeanTween.alphaCanvas(groupCanvas, 1, timeAnimation).uniqueId;
        }
    }


}
