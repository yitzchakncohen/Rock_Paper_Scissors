using System;
using RockPaperScissors.Units;
using UnityEngine;

public class TakingActionState : EnemyState
{
    private EnemyAIAction nextAction = null;
    private TurnManager turnManager;
    private bool takingAction = false;
    private float startActionDelayTimer = 0.5f;

    public TakingActionState(EnemyAIAction nextAction)
    {
        this.nextAction = nextAction;
    }

    public void StartTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager)
    {
        throw new NotImplementedException();
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction, TurnManager turnManager)
    {
        if(takingAction) { return; }

        startActionDelayTimer -= Time.deltaTime;
        if (startActionDelayTimer <= 0f)
        {

            takingAction = true;
            if (TryTakeEnemyAIAction(CompleteAction))
            {
                // Action attempted
            }
            else
            {
                // No more enemies have actions they can take so end the turn. 
                this.turnManager = turnManager;
                EndTurn(context);
            }
        }
    }

    public void CompleteAction(EnemyStateContext context)
    {
        context.SetState(new FindingActionState());
    }

    public void EndTurn(EnemyStateContext context)
    {
        Debug.Log("EndTurn");
        turnManager.NextTurn();
        context.SetState(new WaitingForTurnState());
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        // float startTime = Time.realtimeSinceStartup;

        if (nextAction != null)
        {
            // Debug.Log("Taking action with value: " + bestEnemeyAIAction.actionValue);
            // Debug.Log("Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            if(!nextAction.unitAction.TryTakeAction(nextAction.gridObject, onEnemyAIActionComplete))
            {
                onEnemyAIActionComplete();
            }
            return true;
        }

        // Debug.Log("No Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
        return false;
    }
}