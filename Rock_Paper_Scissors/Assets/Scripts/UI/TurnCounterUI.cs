using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using TMPro;
using UnityEngine;

public class TurnCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    private TurnManager turnManager;

    private void Start() 
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
        turnManager = FindObjectOfType<TurnManager>();
    }

    private void OnDestroy() 
    {
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
    {
        SetTurn();
    }

    private void SaveManager_OnLoadCompleted()
    {
        SetTurn();
    }
    private void SetTurn()
    {
        turnText.text = "Turn: " + turnManager.Turn;
    }

}
