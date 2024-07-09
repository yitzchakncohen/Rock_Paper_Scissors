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
        private TurnManager turnManager;
        private bool waveOccuring = false;

        private void Awake()
        {
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            SaveManager.OnLoadCompleted += SaveManager_OnLoadCompleted;
            UnitManager.OnActionsRemainingUpdated += UnitManager_OnActionsRemainingUpdated;
            button = GetComponent<Button>();
            button.interactable = false;
            button.onClick.AddListener(() => turnManager.NextTurn());
            Debug.Log("next button awake");
        }

        private void Start() 
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        private void OnDestroy() 
        {
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
            SaveManager.OnLoadCompleted -= SaveManager_OnLoadCompleted;
            UnitManager.OnActionsRemainingUpdated -= UnitManager_OnActionsRemainingUpdated;
            button.onClick.RemoveAllListeners();
        }

        private void UnitManager_OnActionsRemainingUpdated(int actionsRemaining)
        {
            if(actionsRemaining <= 0)
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
            if(turnManager.IsPlayerTurn)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }
}
