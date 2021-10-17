using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ButtonSavedGrid : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gridSavedName;
    [SerializeField] private UnityEngine.UI.Button buttonPlay;


    private LevelData levelData;
    private System.Action onPlay;
    public void Init(LevelData levelData, System.Action onPlay)
    {
        this.levelData = levelData;
        this.onPlay = onPlay;
        gridSavedName.text = levelData.name;
        buttonPlay.onClick.AddListener(OnClickPlay);
        gameObject.SetActive(true);
    }

    private void OnClickPlay()
    {
        GameController gameController = ServiceLocator.Instance.GetComponentRegistered<GameController>();
        gameController.PlayGridByLevelData(levelData);
        onPlay?.Invoke();
    }
}
