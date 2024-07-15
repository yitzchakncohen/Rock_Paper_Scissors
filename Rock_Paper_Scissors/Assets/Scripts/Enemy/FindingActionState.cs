using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RockPaperScissors.Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class FindingActionState : EnemyState
{
    bool findingAction = false;
    
    public void StartTurn(EnemyStateContext context)
    {
        throw new NotImplementedException();
    }

    public void FindAction(EnemyStateContext context, UnitManager unitManager, Action<Vector3> OnActionFound)
    {
        FindNextAction(context, unitManager, OnActionFound);
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

    private async void FindNextAction(EnemyStateContext context, UnitManager unitManager, Action<Vector3> OnActionFound)
    {
        // Already finding action
        if (findingAction) { return; }

        // Start the finding action process
        findingAction = true;

        EnemyAIAction nextAction = await GetBestEnemyAction(unitManager);

        if(nextAction != null)
        {
            OnActionFound?.Invoke(nextAction.unitAction.transform.position);
        }

        context.SetState(new TakingActionState(nextAction));
    }

    private async Task<EnemyAIAction>GetBestEnemyAction(UnitManager unitManager)
    {
        List<EnemyAIAction> bestEnemeyAIActions = new List<EnemyAIAction>();

        // Get the best action from each unit and see if it is the best.
        foreach (Unit enemyUnit in unitManager.GetEnemyUnitsList())
        {
            EnemyAIAction testAction = await GetBestActionForUnit(enemyUnit);
            if (testAction != null)
            {
                if(bestEnemeyAIActions.Count > 0)
                {
                    if (testAction.actionValue > bestEnemeyAIActions.First().actionValue)
                    {
                        bestEnemeyAIActions.Clear();
                        bestEnemeyAIActions.Add(testAction);
                    }
                    else if(testAction.actionValue == bestEnemeyAIActions.First().actionValue)
                    {
                        bestEnemeyAIActions.Add(testAction);
                    }
                }
                else
                {
                    bestEnemeyAIActions.Add(testAction);
                }
            }
        }

        return  bestEnemeyAIActions.Count == 0 ? null : bestEnemeyAIActions[Random.Range(0, bestEnemeyAIActions.Count)];
    }

    private async Task<EnemyAIAction> GetBestActionForUnit(Unit enemyUnit)
    {
        await Task.Yield();

        List<EnemyAIAction> bestEnemeyAIActions = new List<EnemyAIAction>();

        foreach (UnitAction baseAction in enemyUnit.UnitActions)
        {
            // float startTime = Time.realtimeSinceStartup;

            if(baseAction.ActionPointsRemaining <= 0 || baseAction.GetTrappedTurnsRemaining() > 0)
            {
                // Enemy cannot afford this action
                continue;
            }

            EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();

            // Find the best of the best.
            if(testEnemyAIAction != null)
            {
                if(bestEnemeyAIActions.Count > 0)
                {
                    if (testEnemyAIAction.actionValue > bestEnemeyAIActions.First().actionValue)
                    {
                        bestEnemeyAIActions.Clear();
                        bestEnemeyAIActions.Add(testEnemyAIAction);
                    }
                    else if(testEnemyAIAction.actionValue == bestEnemeyAIActions.First().actionValue)
                    {
                        bestEnemeyAIActions.Add(testEnemyAIAction);
                    }
                }
                else
                {
                    bestEnemeyAIActions.Add(testEnemyAIAction);
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

        return  bestEnemeyAIActions.Count == 0 ? null : bestEnemeyAIActions[Random.Range(0, bestEnemeyAIActions.Count)];
    }
}