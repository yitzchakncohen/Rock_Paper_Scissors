using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

public class TurnManager : MonoBehaviour
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
        if(playersTurn)
        {
            turn++;
        }
        unitManager.ResetAllUnitActionPoints();
        
        OnNextTurnEventArgs onNextTurnEventArgs = new OnNextTurnEventArgs
        {
            IsPlayersTurn = playersTurn,
            Turn = turn
        };
        OnNextTurn?.Invoke(this, onNextTurnEventArgs);
    }
}
