using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using UnityEngine;
using UnityEngine.UI;

public class AdModal : MonoBehaviour
{
    public static event EventHandler<GameplayManager.OnGameOverEventArgs> OnWatchButtonClick;
    public static event EventHandler<GameplayManager.OnGameOverEventArgs> OnSkipButtonClick;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button skipAdButton;
    private GameplayManager.OnGameOverEventArgs onGameOverEventArgs = null;

    private void Awake() 
    {
        watchAdButton.onClick.AddListener(WatchAdButton_onClick);
        skipAdButton.onClick.AddListener(SkipAdButton_onClick);
    }

    private void SkipAdButton_onClick()
    {
        OnSkipButtonClick?.Invoke(this, onGameOverEventArgs);
        gameObject.SetActive(false);
    }

    private void WatchAdButton_onClick()
    {
        OnWatchButtonClick?.Invoke(this, onGameOverEventArgs);
        gameObject.SetActive(false);
    }

    public void PassGameOverEventArgs(GameplayManager.OnGameOverEventArgs onGameOverEventArgs)
    {
        this.onGameOverEventArgs = onGameOverEventArgs;
    }
}
