using System;
using RockPaperScissors.Units;
using UnityEngine;
using System.Threading.Tasks;
using RockPaperScissors;

public class EnemyAI : MonoBehaviour
{
    private EnemyStatePattern state = new EnemyStatePattern();
    private TurnManager turnManager;
    private UnitManager unitManager;
    private ActionHandler actionHandler;

    private void Start() 
    {
        turnManager = FindObjectOfType<TurnManager>();
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        unitManager = FindObjectOfType<UnitManager>();
        actionHandler = FindObjectOfType<ActionHandler>();
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
    }

    private void Update() 
    {
        if(turnManager.IsPlayerTurn){ return; }
        if(actionHandler.IsBusy()) { return; }
        if(state == null)
        {
            // Used to signify the end of the game.
            return;
        }

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
        if(!turnManager.IsPlayerTurn && state != null)
        {
            state.StartTurn();
        }
    }

    private void CompleteAction()
    {
        if(state != null)
        {
            state.CompleteAction();
        }
    }

    private void GameplayManager_OnGameOver(object sender, EventArgs e)
    {
        // Use a null state to mark the game being over.
        state = null;
    }
}
