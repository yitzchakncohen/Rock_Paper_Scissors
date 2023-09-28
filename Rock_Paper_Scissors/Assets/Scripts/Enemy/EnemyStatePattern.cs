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
    void TakeAction(EnemyStateContext context, Action CompleteAction, TurnManager turnManager);
    void CompleteAction(EnemyStateContext context);
    void EndTurn(EnemyStateContext context);
}

public class EnemyStatePattern : EnemyStateContext
{
    private EnemyState currentState = new WaitingForTurnState();

    public void StartTurn() => currentState.StartTurn(this);
    public void FindAction(UnitManager unitManager) => currentState.FindAction(this, unitManager);
    public void TakeAction(Action CompleteAction, TurnManager turnManager) => currentState.TakeAction(this, CompleteAction, turnManager);
    public void CompleteAction() => currentState.CompleteAction(this);
    public void EndTurn() => currentState.EndTurn(this);

    public void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}
