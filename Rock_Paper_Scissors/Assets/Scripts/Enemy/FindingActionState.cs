using System;
using System.Threading.Tasks;
using RockPaperScissors.Units;
using UnityEngine;

public class FindingActionState : EnemyState
{
    bool findingAction = false;
    
    public void StartTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager)
    {
        FindNextAction(context, unitManager);
    }

    public void TakeAction(EnemyStateContext context, Action CompleteAction, TurnManager turnManager)
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

    private async void FindNextAction(EnemyStateContext context, UnitManager unitManager)
    {
        // Already finding action
        if (findingAction) { return; }

        // Start the finding action process
        findingAction = true;

        EnemyAIAction nextAction = await GetBestEnemyAction(unitManager);

        context.SetState(new TakingActionState(nextAction));
    }

    private async Task<EnemyAIAction>GetBestEnemyAction(UnitManager unitManager)
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
            // float startTime = Time.realtimeSinceStartup;

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

            // if(baseAction is UnitAttack)
            // {
            //     Debug.Log("Attack Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            // }
            // else
            // {
            //     Debug.Log("Move Action Found: " + (Time.realtimeSinceStartup - startTime) * 1000f);
            // }
        }
        return bestEnemeyAIAction;
    }
}