using System;
using RockPaperScissors.Units;

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
}