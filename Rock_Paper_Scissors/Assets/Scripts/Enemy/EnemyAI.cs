using System;
using RockPaperScissors.Units;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    private enum State 
    {
        WaitingForTurn,
        FindingAction,
        TakingTurn, 
        BusyTakingAction,
    }

    private State state;
    private TurnManager turnManager;
    private UnitManager unitManager;
    private EnemyAIAction nextAction = null;
    private bool nextActionFound = false;
    private bool findingNextAction = false;
    private float timer;

    private void Awake() 
    {
        state = State.WaitingForTurn;
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
            case State.WaitingForTurn:
                break;
            case State.FindingAction:
                FindAction();
                break;
            case State.TakingTurn:
                TakeAction();
                break;
            case State.BusyTakingAction:
                break;
        }
    }

    private void FindAction()
    {
        // Debug.Log("AI searching for action...");
        if (nextActionFound)
        {
            state = State.TakingTurn;
        }
        else
        {
            if (!findingNextAction)
            {
                FindNextAction();
            }
        }
    }
    
    private void TakeAction()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (TryTakeEnemyAIAction(CompleteAction))
            {
                state = State.BusyTakingAction;
            }
            else
            {
                // No more enemies have actions they can take so end the turn. 
                turnManager.NextTurn();
            }
        }
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs eventArgs)
    {
        if(!turnManager.IsPlayerTurn())
        {
            nextActionFound = false;
            state = State.FindingAction;
        }
    }

    private void CompleteAction()
    {
        timer = 0.5f;
        nextActionFound = false;
        state = State.FindingAction;
    }

    private async void FindNextAction()
    {
        findingNextAction = true;
        
        nextAction = await GetBestEnemyAction();

        findingNextAction = false;
        nextActionFound = true;
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
