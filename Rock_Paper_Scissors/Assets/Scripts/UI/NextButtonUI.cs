using System;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class NextButtonUI : MonoBehaviour
    {
        [SerializeField] private GameObject highlight;
        private Button button;
        private UnitManager unitManager;

        private void Start() 
        {
            unitManager = FindObjectOfType<UnitManager>();
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            button = GetComponent<Button>();
            button.interactable = false;
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            CheckActionsRemainingAsync();
        }

        private async void CheckActionsRemainingAsync()
        {
            // float startTime = Time.realtimeSinceStartup;

            int actionsRemaining = await unitManager.GetFriendlyAvaliableActionsRemaining();
            if(actionsRemaining <= 0)
            {
                highlight.SetActive(true);
            }
            else
            {
                highlight.SetActive(false);
            }

            // Debug.Log("NextButtonUI Action Complete Time: " + (Time.realtimeSinceStartup - startTime)*1000f);
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
        {
            highlight.SetActive(false);
            if(e.IsPlayersTurn)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }            
        }

        private void WaveManager_OnWaveCompleted()
        {
            button.interactable = true;
        }

        private void WaveManager_OnWaveStarted()
        {
            button.interactable = false;
        }
    }
}
