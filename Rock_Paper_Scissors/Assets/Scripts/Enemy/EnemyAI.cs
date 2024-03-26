using System;
using RockPaperScissors.Units;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    private EnemyStatePattern state = new EnemyStatePattern();
    private TurnManager turnManager;
    private UnitManager unitManager;

    private void Start() 
    {
        turnManager = FindObjectOfType<TurnManager>();
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        unitManager = FindObjectOfType<UnitManager>();
    }

    private void Update() 
    {
        if(turnManager.IsPlayerTurn()){ return; }

        switch (state.GetCurrentState())
        {
            case WaitingForTurnState:
                break;
            case FindingActionState:
                state.FindAction(unitManager);
                break;
            case TakingActionState:
                state.TakeAction(CompleteAction, turnManager);
                break;
        }
    }

    private void TurnManager_OnNextTurn(object sender, EventArgs eventArgs)
    {
        if(!turnManager.IsPlayerTurn())
        {
            state.StartTurn();
        }
    }

    private void CompleteAction()
    {
        state.CompleteAction();
    }
}
