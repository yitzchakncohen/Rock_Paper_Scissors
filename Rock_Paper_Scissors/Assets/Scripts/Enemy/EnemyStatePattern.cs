using System;
using System.Threading.Tasks;
using RockPaperScissors.Units;
using UnityEngine;

public interface EnemyStateContext
{
    void SetState(EnemyState newState);
}

public interface EnemyState
{
    void StartTurn(EnemyStateContext context);
    void FindAction(EnemyStateContext context, UnitManager unitManager);
    void TakeAction(EnemyStateContext context, Action CompleteAction);
    void CompleteAction(EnemyStateContext context);
    void EndTurn(EnemyStateContext context);
}

public class EnemyStatePattern : MonoBehaviour, EnemyStateContext
{
    EnemyState currentState = new WaitingForTurnState();

    public void StartTurn() => currentState.StartTurn(this);
    public void FindAction(UnitManager unitManager) => currentState.FindAction(this, unitManager);
    public void TakeAction(Action CompleteAction) => currentState.TakeAction(this, CompleteAction);
    public void CompleteAction() => currentState.CompleteAction(this);
    public void EndTurn() => currentState.EndTurn(this);

    public void SetState(EnemyState newState)
    {
        currentState = newState;
    }
}

public class WaitingForTurnState : EnemyState
{
    public void StartTurn(EnemyStateContext context)
    {
        context.SetState(new FindingActionState());
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager)
    {
        throw new NotImplementedException();
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        throw new NotImplementedException();
    }

    public void CompleteAction(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void EndTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }
}

public class FindingActionState : EnemyState
{
    private UnitManager unitManager;
    
    public void StartTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager)
    {
        this.unitManager = unitManager;

        FindNextAction(context);
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        throw new NotImplementedException();
    }

    public void CompleteAction(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void EndTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    private async void FindNextAction(EnemyStateContext context)
    {
        EnemyAIAction nextAction = await GetBestEnemyAction();

        context.SetState(new TakingActionState(nextAction));
    }

    private async Task<EnemyAIAction>GetBestEnemyAction()
    {
        EnemyAIAction bestEnemeyAIAction = null;

        // Get the best action from each unit and see if it is the best.
        foreach (Unit enemyUnit in unitManager.GetEnemyUnitsList())
        {
            if (bestEnemeyAIAction == null)
            {
                bestEnemeyAIAction = await GetBestActionForUnit(enemyUnit);
            }
            else
            {
                EnemyAIAction testAction = await GetBestActionForUnit(enemyUnit);
                if (testAction != null && testAction.actionValue > bestEnemeyAIAction.actionValue)
                {
                    bestEnemeyAIAction = testAction;
                }
            }
        }

        return bestEnemeyAIAction;
    }

    private async Task<EnemyAIAction> GetBestActionForUnit(Unit enemyUnit)
    {
        await Task.Yield();

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

public class TakingActionState : EnemyState
{
    private EnemyAIAction nextAction = null;
    private TurnManager turnManager;
    private float timer = 0.5f;

    public TakingActionState(EnemyAIAction nextAction)
    {
        this.nextAction = nextAction;
    }

    public void StartTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void CompleteAction(EnemyStateContext context)
    {
        context.SetState(new FindingActionState());
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager)
    {
        throw new NotImplementedException();
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {

            if (TryTakeEnemyAIAction(CompleteAction))
            {
                // Action successfully taken
            }
            else
            {
                // No more enemies have actions they can take so end the turn. 
                EndTurn(context);
            }
        }
    }

    public void EndTurn(EnemyStateContext context)
    {
        turnManager.NextTurn();
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        // float startTime = Time.realtimeSinceStartup;

        if (nextAction != null)
        {
            // Debug.Log("Taking action with value: " + bestEnemeyAIAction.actionValue);
            // Debug.Log("Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            return nextAction.unitAction.TryTakeAction(nextAction.gridObject, onEnemyAIActionComplete);
        }

        // Debug.Log("No Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
        return false;
    }
}

