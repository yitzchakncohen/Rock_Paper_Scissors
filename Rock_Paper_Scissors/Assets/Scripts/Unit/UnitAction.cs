using System;
using RockPaperScissors.Grids;
using RockPaperScissors.UI;
using UnityEngine;

namespace RockPaperScissors.Units
{
    // Base action class which the unit actions inherit from.
    public abstract class UnitAction : MonoBehaviour
    {
        public static event EventHandler OnAnyActionStarted;
        public static event EventHandler OnAnyActionCompleted;
        public bool IsCancellableAction {get; protected set;}
        protected bool isActive;
        protected Action onActionComplete;
        protected int actionPointsRemaining = 1;

        protected virtual void Start() 
        {
            CancelButton.OnCancelButtonPress += CancelButton_OnCancelButtonPress;
        }

        private void OnDestroy() 
        {
            CancelButton.OnCancelButtonPress -= CancelButton_OnCancelButtonPress;
        }

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

        protected virtual void CancelButton_OnCancelButtonPress()
        {
            if(IsCancellableAction)
            {
                ActionComplete();
            }
        }

        public int GetActionPointsRemaining()
        {
            return actionPointsRemaining;
        }

        public void ResetActionPoints()
        {
            actionPointsRemaining = 1;
        }

        public abstract EnemyAIAction GetBestEnemyAIAction();
        public abstract bool TryTakeAction(GridObject gridObject, Action onActionComplete);
        public abstract int GetValidActionsRemaining();
    }
}
