using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    private UnitManager unitManager;

    private void Start() 
    {
        unitManager = FindObjectOfType<UnitManager>();
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
    }

    private void OnDestroy() 
    {
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
        UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
    }

    private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        if(unitManager.GetFriendlyAvaliableActionsRemaining() <= 0)
        {
            highlight.SetActive(true);
        }
        else
        {
            highlight.SetActive(false);
        }
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
    {
        highlight.SetActive(false);
    }
}
