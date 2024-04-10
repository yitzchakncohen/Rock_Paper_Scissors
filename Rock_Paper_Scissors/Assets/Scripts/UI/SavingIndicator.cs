using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI.Buttons;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;

public class SavingIndicator : MonoBehaviour
{
    private const string LAST_SAVE_PREF = "lastSave";
    private const string DEFAULT_SAVE_TIME = "never";
    [SerializeField] private LoadingSpinner loadingSpinner;
    [SerializeField] private LetterAnimation lastSaveText;

    void Awake()
    {
        SaveButton.OnSaveButtonPress += SaveButton_OnSaveButtonPress;
    }

    private void OnEnable() 
    {
        SaveManager.OnSaveCompleted += SaveManager_OnSaveCompleted;
        UpdateLastSaveTime();
        loadingSpinner.gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        SaveManager.OnSaveCompleted -= SaveManager_OnSaveCompleted;
    }

    private void UpdateLastSaveTime()
    {
        string lastSaveString = PlayerPrefs.GetString(LAST_SAVE_PREF, DEFAULT_SAVE_TIME);
        string timeString = $"Last Save: {lastSaveString} min ago";
        if(lastSaveString != DEFAULT_SAVE_TIME)
        {
            DateTime lastSaveTime = DateTime.Parse(lastSaveString);
            double timeInMinutes =  DateTime.UtcNow.Subtract(lastSaveTime).TotalMinutes;
            timeString = $"Last Save: {timeInMinutes.ToString("N0")} min ago";
        }
        lastSaveText.Play(timeString);
    }

    private void SaveButton_OnSaveButtonPress()
    {
        loadingSpinner.gameObject.SetActive(true);
    }

    private void SaveManager_OnSaveCompleted()
    {
        loadingSpinner.gameObject.SetActive(false);
        string saveTime = DateTime.UtcNow.ToString();
        PlayerPrefs.SetString(LAST_SAVE_PREF, saveTime);
        UpdateLastSaveTime();
    }
}
