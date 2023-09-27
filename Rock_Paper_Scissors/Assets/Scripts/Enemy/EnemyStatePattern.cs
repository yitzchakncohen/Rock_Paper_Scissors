using System;
using System.Threading;
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
    void FindAction(EnemyStateContext context, UnitManager unitManager, bool nextActionFound, bool findingNextAction);
    void TakeAction(EnemyStateContext context, Action CompleteAction);
    void CompleteAction(EnemyStateContext context);
    void EndTurn(EnemyStateContext context);
}

public class EnemyStatePattern : MonoBehaviour, EnemyStateContext
{
    EnemyState currentState = new WaitingForTurnState();

    public void StartTurn() => currentState.StartTurn(this);
    public void FindAction(UnitManager unitManager, bool nextActionFound, bool findingNextAction) => currentState.FindAction(this, unitManager ,nextActionFound, findingNextAction);
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
    public void CompleteAction(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void EndTurn(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager, bool nextActionFound, bool findingNextAction)
    {
        throw new NotImplementedException();
    }

    public void StartTurn(EnemyStateContext context)
    {
        context.SetState(new FindingActionState());
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        throw new NotImplementedException();
    }
}

public class FindingActionState : EnemyState
{
    private bool nextActionFound = false;
    private bool findingNextAction = false;
    private EnemyAIAction nextAction = null;
    private UnitManager unitManager;
    private float timer = 0.5f;
    
    public void CompleteAction(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void EndTurn(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager, bool nextActionFound, bool findingNextAction)
    {
        this.nextActionFound = nextActionFound;
        this.findingNextAction = findingNextAction;
        this.unitManager = unitManager;

        // Debug.Log("AI searching for action...");
        if (nextActionFound)
        {
            context.SetState(new TakingActionState());
        }
        else
        {
            if (!findingNextAction)
            {
                FindNextAction();
            }
        }
    }

    public void StartTurn(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TakingActionState takingActionState = new TakingActionState();

            if (TryTakeEnemyAIAction(CompleteAction))
            {
                context.SetState(takingActionState);
            }
            else
            {
                // No more enemies have actions they can take so end the turn. 
                EndTurn(context);
            }
        }
    }

    private async void FindNextAction()
    {
        findingNextAction = true;
        
        nextAction = await GetBestEnemyAction();

        findingNextAction = false;
        nextActionFound = true;
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

public class TakingActionState : EnemyState
{
    private TurnManager turnManager;
    public void CompleteAction(EnemyStateContext context)
    {
        context.SetState(new FindingActionState());
    }

    public void EndTurn(EnemyStateContext context)
    {
        turnManager.NextTurn();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager, bool nextActionFound, bool findingNextAction)
    {
        throw new NotImplementedException();
    }

    public void StartTurn(EnemyStateContext context)
    {
        throw new System.NotImplementedException();
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction)
    {
        throw new NotImplementedException();
    }
}

