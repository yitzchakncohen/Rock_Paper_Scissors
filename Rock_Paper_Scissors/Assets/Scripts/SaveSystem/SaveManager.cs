using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public const string SAVE_DIRECTORY = "/Saves/";
        public const string SAVE_FILE_NAME = "save.txt";
        public static event Action OnSaveCompleted; 
        public static event Action OnLoadCompleted; 
        [SerializeField] private List<Unit> listOfFriendlyUnitTypes = new List<Unit>();
        [SerializeField] private List<Unit> listOfEnemyUnitTypes = new List<Unit>();
        private Dictionary<UnitClass, Unit> dictionaryOfFriendlyUnitTypes = new Dictionary<UnitClass, Unit>();
        private Dictionary<UnitClass, Unit> dictionaryOfEnemyUnitTypes = new Dictionary<UnitClass, Unit>();
        private GridManager gridManager;
        private TurnManager turnManager;
        private CurrencyBank currencyBank;
        private GameplayManager gameplayManager;

        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
            turnManager = FindObjectOfType<TurnManager>();
            currencyBank = FindObjectOfType<CurrencyBank>();
            gameplayManager = FindObjectOfType<GameplayManager>();
            CheckForDirectory();

            SetupUnitDictionaries();
        }

        private static void CheckForDirectory()
        {
            if (!Directory.Exists(Application.persistentDataPath + SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(Application.persistentDataPath + SAVE_DIRECTORY);
                Debug.Log("Save Directory Created");
            }
        }

        private void SetupUnitDictionaries()
        {
            foreach (Unit unit in listOfFriendlyUnitTypes)
            {
                dictionaryOfFriendlyUnitTypes.Add(unit.Class, unit);
            }
            foreach (Unit unit in listOfEnemyUnitTypes)
            {
                dictionaryOfEnemyUnitTypes.Add(unit.Class, unit);
            }
        }

        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            StartCoroutine(SaveGameAsync());
        }

        private IEnumerator SaveGameAsync()
        {
            // TODO add saving indicator

            List<SaveUnitData> SaveUnitDataList = new List<SaveUnitData>();

            foreach (var unit in FindObjectsOfType<Unit>())
            {
                // Get Unit location
                SaveUnitData saveUnitData = unit.Save();
                saveUnitData.GridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
                SaveUnitDataList.Add(saveUnitData);
            }


            SaveCurrencyBankData currencyBankData = currencyBank.Save();
            SaveTurnManagerData turnManagerData = turnManager.Save();
            SaveGameplayManagerData saveGameManagerData = gameplayManager.Save();

            SaveObject saveObject = new SaveObject
            {
                SaveCurrencyBankData = currencyBankData,
                SaveTurnManagerData = turnManagerData,
                UnitList = SaveUnitDataList,
                SaveGameManagerData = saveGameManagerData,
            };

            string json = JsonUtility.ToJson(saveObject);
            CheckForDirectory();
            File.WriteAllText(Application.persistentDataPath + SAVE_DIRECTORY + SAVE_FILE_NAME, json);

            yield return null;

            OnSaveCompleted?.Invoke();
        }

        // For Debugging Only
        // [ContextMenu("Load Game")]
        // public void LoadGame()
        // {
        //     LoadGameAsync();
        // }

        public async Task LoadGameAsync()
        {
            await Task.Yield();
            // TODO clear all grid objects and delete all units. 

            if (File.Exists(Application.persistentDataPath + SAVE_DIRECTORY + "save.txt"))
            {
                string saveString = File.ReadAllText(Application.persistentDataPath + SAVE_DIRECTORY + "save.txt");
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                turnManager.Load(saveObject.SaveTurnManagerData);
                currencyBank.Load(saveObject.SaveCurrencyBankData);
                gameplayManager.Load(saveObject.SaveGameManagerData);
                foreach (SaveUnitData unitData in saveObject.UnitList)
                {
                    SpawnUnitByClassandTeam(unitData);
                }
            }
            else
            {
                Debug.LogError("No save file found.");
            }
            gridManager.UpdateGridOccupancy();

            OnLoadCompleted?.Invoke();
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
                    Debug.LogError("Cannot load Friendly Unit, " + unitData.UnitClass.ToString() + " class not found.");
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
