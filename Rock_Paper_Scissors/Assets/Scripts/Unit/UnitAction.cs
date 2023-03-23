using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    protected bool isActive;
    protected Action onActionComplete;
    protected int actionPointsRemaining = 1;
    
    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public abstract EnemyAIAction GetBestEnemyAIAction();
    public abstract bool TryTakeAction(GridObject gridObject, Action onActionComplete);

    public int GetActionPointsRemaining()
    {
        return actionPointsRemaining;
    }

    public void ResetActionPoints()
    {
        actionPointsRemaining = 1;
    }

}