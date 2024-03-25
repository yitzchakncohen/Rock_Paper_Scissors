using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

public class TurnManager : MonoBehaviour, ISaveInterface<SaveTurnManagerData>
{
    public class OnNextTurnEventArgs : EventArgs
    {
        public bool IsPlayersTurn;
        public int Turn;
    }
    public static event EventHandler<OnNextTurnEventArgs> OnNextTurn;
    private UnitManager unitManager;
    private bool playersTurn = true;
    private int turn = 0;

    private void Start() 
    {
        unitManager = FindObjectOfType<UnitManager>();
    }


    public bool IsPlayerTurn()
    {
        return playersTurn;
    }

    public void NextTurn()
    {
        playersTurn = !playersTurn;
        if (playersTurn)
        {
            turn++;
        }
        unitManager.ResetAllUnitActionPoints();

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
            IsPlayersTurn = IsPlayerTurn(),
            Turn = turn
        };

        return turnManagerData;
    }

    public void Load(SaveTurnManagerData loadData)
    {
        playersTurn = loadData.IsPlayersTurn;
        turn = loadData.Turn;
    }
}
