using System;
using System.Collections;
using RockPaperScissors.Units;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class StatusUpdateUI : MonoBehaviour
    {
        private const string ENEMY_WAVE = "Enemy Wave Incoming...";
        private const string PLAYER_TURN = "Your Turn";
        private const string ENEMY_TURN = "Enemy Turn";
        private const string MOVING = "Moving...";
        private const string ATTACKING = "Attacking...";
        private const string PLACING_UNIT = "Placing unit...";
        private const string GLUE_TRAP = "It's a Trap!";
        private const string TRAMPOLINE_TRAP = "Boooiiinnnng!";
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject background;

        private void Start() 
        {
            UnitAction.OnAnyActionStarted += UnitAction_OnAnyActionStarted;
            UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
            TurnManager.OnNextTurn += TurnManager_OnNextTurn;
            WaveManager.OnWaveStarted += WaveManager_OnWaveStarted;
            WaveManager.OnWaveCompleted += WaveManager_OnWaveCompleted;
            HideStatus();
        }

        
        private void OnDestroy() 
        {
            UnitAction.OnAnyActionStarted -= UnitAction_OnAnyActionStarted;
            UnitAction.OnAnyActionCompleted -= UnitAction_OnAnyActionCompleted;
            TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
            WaveManager.OnWaveStarted -= WaveManager_OnWaveStarted;
            WaveManager.OnWaveCompleted -= WaveManager_OnWaveCompleted;
        }

        private void WaveManager_OnWaveCompleted()
        {
            HideStatus();
        }

        private void WaveManager_OnWaveStarted()
        {
            ShowStatus(ENEMY_WAVE, 0);
        }

        private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
        {
            if(eventArgs.IsPlayersTurn)
            {
                ShowStatus(PLAYER_TURN, 5);
            }
            else
            {
                ShowStatus(ENEMY_TURN, 5);
            }
        }

        private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            // float startTime = Time.realtimeSinceStartup;

            HideStatus();
            // Debug.Log("StatusUpdateUI Action Complete Time: " + (Time.realtimeSinceStartup - startTime)*1000f);
        }


        private void UnitAction_OnAnyActionStarted(object sender, EventArgs eventArgs)
        {
            switch(sender)
            {
                case (UnitMovement unitMovement):
                    ShowStatus(MOVING, 0);
                    break;
                case (UnitAttack unitAttack):
                    ShowStatus(ATTACKING, 0);
                    break;
                case (UnitSpawner unitSpawner):
                    ShowStatus(PLACING_UNIT, 0);
                    break;
                case(UnitTrap unitTrap):
                    if(unitTrap as GlueTrap)
                    {
                        ShowStatus(GLUE_TRAP, 0);
                    }
                    else if(unitTrap as TrampolineTrap)
                    {
                        ShowStatus(TRAMPOLINE_TRAP, 0);
                    }
                    break;
            }
        }

        private void ShowStatus(string status, float statusDisplayTime)
        {
            background.SetActive(true);
            statusText.text = status;
            if(statusDisplayTime > 0)
            {
                StartCoroutine(HideStatusRoutine(statusDisplayTime));
            }
        }

        private IEnumerator HideStatusRoutine(float statusDisplayTime)
        {
            yield return new WaitForSeconds(statusDisplayTime);
            HideStatus();
        }

        private void HideStatus()
        {
            background.SetActive(false);
        }
    }    
}
