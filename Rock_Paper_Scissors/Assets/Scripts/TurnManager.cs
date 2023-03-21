using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static event EventHandler OnNextTurn;
    private UnitManager unitManager;
    private bool playersTurn = true;

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
        unitManager.ResetAllUnitActionPoints();
        OnNextTurn?.Invoke(this, EventArgs.Empty);
    }
}
