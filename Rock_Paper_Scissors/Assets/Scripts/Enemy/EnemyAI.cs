using System;
using RockPaperScissors.Units;
using RockPaperScissors.Grids;
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
                    }
                    else
                    {
                        // No more enemies have actions they can take so end the turn. 
                        turnManager.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs eventArgs)
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
        EnemyAIAction bestEnemeyAIAction = null;

        // Get the best action from each unit and see if it is the best.
        foreach (Unit enemyUnit in unitManager.GetEnemyUnitsList())
        {
            if(bestEnemeyAIAction == null)
            {
                bestEnemeyAIAction = GetBestActionForUnit(enemyUnit, onEnemyAIActionComplete);
            }
            else
            {
                EnemyAIAction testAction = GetBestActionForUnit(enemyUnit, onEnemyAIActionComplete);
                if(testAction != null && testAction.actionValue > bestEnemeyAIAction.actionValue)
                {
                    bestEnemeyAIAction = testAction;
                }
            }
        }

        if(bestEnemeyAIAction != null)
        {
            // Debug.Log("Taking action with value: " + bestEnemeyAIAction.actionValue);
            return bestEnemeyAIAction.unitAction.TryTakeAction(bestEnemeyAIAction.gridObject, onEnemyAIActionComplete);
        }
        return false;
    }

    private EnemyAIAction GetBestActionForUnit(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemeyAIAction = null;

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
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemeyAIAction.actionValue)
                {
                    bestEnemeyAIAction = testEnemyAIAction;
                }
            }
        }
        return bestEnemeyAIAction;

    }
}
