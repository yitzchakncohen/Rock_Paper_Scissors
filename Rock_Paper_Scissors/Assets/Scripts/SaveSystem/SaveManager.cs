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
                string json = JsonUtility.ToJson(unit.Save());
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
