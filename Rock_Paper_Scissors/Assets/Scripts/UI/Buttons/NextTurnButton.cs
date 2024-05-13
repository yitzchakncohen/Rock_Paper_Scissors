using System;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Buttons
{
    public class NextTurnButton : MonoBehaviour
    {
        [SerializeField] private GameObject highlight;
        private Button button;
        private UnitManager unitManager;
        private TurnManager turnManager;
        private bool waveOccuring = false;

        private void Awake()
        {
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
            button = GetComponent<Button>();
            button.interactable = false;
            button.onClick.AddListener(() => turnManager.NextTurn());
            Debug.Log("next button awake");
        }

        private void Start() 
        {
            unitManager = FindObjectOfType<UnitManager>();
            turnManager = FindObjectOfType<TurnManager>();
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
            SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
            button.onClick.RemoveAllListeners();
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            if(turnManager.IsPlayerTurn)
            {
                CheckActionsRemainingAsync();
            }
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
            if(e.IsPlayersTurn && !waveOccuring)
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
            waveOccuring = false;
        }

        private void WaveManager_OnWaveStarted()
        {
            button.interactable = false;
            waveOccuring = true;
        }

        private void SaveManager_OnLoadCompleted()
        {
            button.interactable = true;
            if(turnManager.IsPlayerTurn)
            {
                CheckActionsRemainingAsync();
            }
        }
    }
}
