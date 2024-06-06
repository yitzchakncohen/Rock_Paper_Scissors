using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using UnityEngine;

public class TurnManager : MonoBehaviour, ISaveInterface<SaveTurnManagerData>
{
    public class OnNextTurnEventArgs : EventArgs
    {
        public bool IsPlayersTurn;
        public int Turn;
    }
    public static event EventHandler<OnNextTurnEventArgs> OnNextTurn;
    private bool playersTurn = true;
    private int turn = 0;

    public int Turn => turn;
    public bool IsPlayerTurn => playersTurn;


    private void Start() 
    {
        SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
    }

    private void OnDestroy() 
    {
        SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
    }

    public void NextTurn()
    {
        playersTurn = !playersTurn;
        if (playersTurn)
        {
            turn++;
        }

        NextTurnEvent();
    }

    private void NextTurnEvent()
    {
        OnNextTurnEventArgs onNextTurnEventArgs = new OnNextTurnEventArgs
        {
            IsPlayersTurn = playersTurn,
            Turn = turn
        };
        OnNextTurn?.Invoke(this, onNextTurnEventArgs);
    }

    public SaveTurnManagerData Save()
    {
        SaveTurnManagerData turnManagerData = new SaveTurnManagerData
        {
            IsPlayersTurn = this.IsPlayerTurn,
            Turn = turn
        };

        return turnManagerData;
    }

    public void Load(SaveTurnManagerData loadData)
    {
        playersTurn = loadData.IsPlayersTurn;
        turn = loadData.Turn;
    }

    private void SaveManager_OnLoadCompleted()
    {
        OnNextTurnEventArgs onNextTurnEventArgs = new OnNextTurnEventArgs
        {
            IsPlayersTurn = playersTurn,
            Turn = turn
        };
        OnNextTurn?.Invoke(this, onNextTurnEventArgs);
    }
}
