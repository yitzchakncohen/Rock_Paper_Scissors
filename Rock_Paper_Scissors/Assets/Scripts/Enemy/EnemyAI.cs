using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State 
    {
        Waiting,
        TakingTurn, 
        Busy,
    }

    private State state;
    private TurnManager turnManager;
    private UnitManager unitManager;
    private float timer;

    private void Awake() 
    {
        state = State.Waiting;
    }

    private void Start() 
    {
        turnManager = FindObjectOfType<TurnManager>();
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        unitManager = FindObjectOfType<UnitManager>();
    }

    private void Update() 
    {
        if(turnManager.IsPlayerTurn()){ return; }

        switch (state)
        {
            case State.Waiting:
                break;
            case State.TakingTurn:
                timer-= Time.deltaTime;
                if(timer <= 0f)
                {
                    if(TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                        Debug.Log("Busy");
                    }
                    else
                    {
                        // No more enemies have actions they can take so end the turn. 
                        Debug.Log("Next Turn");
                        turnManager.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs e)
    {
        if(!turnManager.IsPlayerTurn())
        {
            state = State.TakingTurn;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in unitManager.GetEnemyUnitsList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemeyAIAction = null;
        UnitAction bestUnitAction = null;

        foreach (UnitAction baseAction in enemyUnit.GetUnitActions())
        {
            if(baseAction.GetActionPointsRemaining() <= 0)
            {
                // Enemy cannot afford this action
                continue;
            }

            // Find the best of the best.
            if(bestEnemeyAIAction == null)
            {
                bestEnemeyAIAction = baseAction.GetBestEnemyAIAction();
                bestUnitAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemeyAIAction.actionValue)
                {
                    bestEnemeyAIAction = testEnemyAIAction;
                    bestUnitAction = baseAction;
                }
            }
        }

        if(bestEnemeyAIAction != null)
        {
            return bestUnitAction.TryTakeAction(bestEnemeyAIAction.gridObject, onEnemyAIActionComplete);
        }
        return false;
    }
}
