using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        private const string SAVE_DIRECTORY = "/Saves/";
        [SerializeField] private List<Unit> listOfFriendlyUnitTypes = new List<Unit>();
        [SerializeField] private List<Unit> listOfEnemyUnitTypes = new List<Unit>();
        private Dictionary<UnitClass, Unit> dictionaryOfFriendlyUnitTypes = new Dictionary<UnitClass, Unit>();
        private Dictionary<UnitClass, Unit> dictionaryOfEnemyUnitTypes = new Dictionary<UnitClass, Unit>();
        private GridManager gridManager;
        private TurnManager turnManager;
        private CurrencyBank currencyBank;

        private void Awake() 
        {
            gridManager = FindObjectOfType<GridManager>();
            turnManager = FindObjectOfType<TurnManager>();
            currencyBank =FindObjectOfType<CurrencyBank>();

            if(!Directory.Exists(Application.dataPath + SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(Application.dataPath + SAVE_DIRECTORY);
            }

            SetupUnitDictionaries();
        }

        private void SetupUnitDictionaries()
        {
            foreach (Unit unit in listOfFriendlyUnitTypes)
            {
                dictionaryOfFriendlyUnitTypes.Add(unit.GetUnitClass(), unit);
            }
            foreach (Unit unit in listOfEnemyUnitTypes)
            {
                dictionaryOfEnemyUnitTypes.Add(unit.GetUnitClass(), unit);
            }
        }

        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            // TODO add saving indicator

            List<SaveUnitData> SaveUnitDataList = new List<SaveUnitData>();

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

                SaveUnitDataList.Add(saveUnitData);
            }
            

            SaveCurrencyBankData currencyBankData = currencyBank.Save();
            SaveTurnManagerData turnManagerData = turnManager.Save();

            SaveObject saveObject = new SaveObject
            {
                SaveCurrencyBankData = currencyBankData,
                SaveTurnManagerData = turnManagerData,
                UnitList = SaveUnitDataList
            };

            string json = JsonUtility.ToJson(saveObject);
            File.WriteAllText(Application.dataPath + SAVE_DIRECTORY + "save.txt", json);
        }

        [ContextMenu("Load Game")]
        public void LoadGame()
        {
            // TODO add loading indicator

            if(File.Exists(Application.dataPath + SAVE_DIRECTORY + "save.txt"))
            {
                string saveString = File.ReadAllText(Application.dataPath + SAVE_DIRECTORY + "save.txt");
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                turnManager.Load(saveObject.SaveTurnManagerData);
                currencyBank.Load(saveObject.SaveCurrencyBankData);
                foreach (SaveUnitData unitData in saveObject.UnitList)
                {
                    SpawnUnitByClassandTeam(unitData);
                }
            }
            else
            {
                Debug.LogError("No save file found.");
            }
        }

        private void SpawnUnitByClassandTeam(SaveUnitData unitData)
        {
            if(unitData.IsFriendly)
            {
                Unit spawnedUnit = null;
                foreach (KeyValuePair<UnitClass, Unit> unit in dictionaryOfFriendlyUnitTypes)
                {
                    if(unitData.UnitClass == unit.Key)
                    {
                        Vector2Int spawnPosition = unitData.GridPosition;
                        spawnedUnit = Instantiate(unit.Value, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                        spawnedUnit.Load(unitData);
                        break;
                    }
                }
                if(spawnedUnit == null)
                {
                    Debug.LogError("Cannot load Frinedly Unit, " + unitData.UnitClass.ToString() + " class not found.");
                }
            }
            else
            {
                Unit spawnedUnit = null;
                foreach (KeyValuePair<UnitClass, Unit> unit in dictionaryOfEnemyUnitTypes)
                {
                    if(unitData.UnitClass == unit.Key)
                    {
                        Vector2Int spawnPosition = unitData.GridPosition;
                        spawnedUnit = Instantiate(unit.Value, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                        spawnedUnit.Load(unitData);
                        break;
                    }
                }
                if(spawnedUnit == null)
                {
                    Debug.LogError("Cannot load Enemy Unit, " + unitData.UnitClass.ToString() + " class not found.");
                }
            }
        }
    }
}
