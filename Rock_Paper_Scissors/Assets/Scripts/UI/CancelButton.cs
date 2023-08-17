using System;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class CancelButton : MonoBehaviour
    {
        public static event Action OnCancelButtonPress;
        private Button button;

        private void Awake() 
        {  
            button = GetComponent<Button>();
        }

        private void Start() 
        {
            UnitAction.OnAnyActionStarted += UnitAction_OnAnyActionStarted;
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            button.interactable = false;
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            // float startTime = Time.realtimeSinceStartup;

            if(((UnitAction)sender).IsCancellableAction)
            {
                button.interactable = false;
            }

            // Debug.Log("CancelButton Action Complete Time: " + (Time.realtimeSinceStartup - startTime)*1000f);
        }

        private void UnitAction_OnAnyActionStarted(object sender, EventArgs e)
        {
            if(((UnitAction)sender).IsCancellableAction)
            {
                button.interactable = true;
            }
        }

        public void CancelButtonPress()
        {
            OnCancelButtonPress?.Invoke();
        }
    }
}
