using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button buttonPause;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private float timeAnimation = 0.3f;
    [SerializeField] private GameObject content;
    [SerializeField] private CanvasGroup groupCanvas;

    bool isPaused;

    int idAnimation;
    // Start is called before the first frame update
    void Start()
    {
        buttonPause.onClick.AddListener(OnClickPause);
        buttonPlay.onClick.AddListener(OnClickPause);
    }

    private void OnClickPause()
    {
        isPaused = !isPaused;

        LeanTween.cancel(idAnimation);

        if (!isPaused)
        {
            idAnimation = LeanTween.alphaCanvas(groupCanvas, 0, timeAnimation)
                .setOnComplete(()=> content.SetActive(false))
                .uniqueId;
        }
        else
        {
            content.SetActive(true);
            idAnimation = LeanTween.alphaCanvas(groupCanvas, 1, timeAnimation).uniqueId;
        }
    }


}
