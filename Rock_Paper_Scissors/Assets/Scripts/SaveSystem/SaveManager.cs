using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        private GridManager gridManager;
        private TurnManager turnManager;
        private CurrencyBank currencyBank;

        private void Awake() 
        {
            gridManager = FindObjectOfType<GridManager>();
            turnManager = FindObjectOfType<TurnManager>();
            currencyBank =FindObjectOfType<CurrencyBank>();
        }

        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            // TODO add saving indicator

            List<string> jsonStrings = new List<string>();

            foreach (var unit in FindObjectsOfType<Unit>())
            {
                // Get Unit location
                SaveUnitData saveUnitData = unit.Save();
                saveUnitData.GridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
                // Get Unit action points
                foreach (var action in unit.GetUnitActions())
                {
                    if(action is UnitAttack)
                    {
                        saveUnitData.AttackActionPointsRemaining = action.GetActionPointsRemaining();
                    }
                    else if(action is UnitMovement)
                    {
                        saveUnitData.MoveActionPointsRemaining = action.GetActionPointsRemaining();
                    }
                }

                string json = JsonUtility.ToJson(saveUnitData);
                jsonStrings.Add(json);
                Debug.Log(json);
            }

            string currencyBankJson = JsonUtility.ToJson(currencyBank.Save());
            jsonStrings.Add(currencyBankJson);

            string turnManagerJson = JsonUtility.ToJson(turnManager.Save());
            jsonStrings.Add(turnManagerJson);
        }

        [ContextMenu("Load Game")]
        public void LoadGame()
        {
            // TODO add loading indicator
        }
    }
}
