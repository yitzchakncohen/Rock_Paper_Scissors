using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

public class WaveManager : MonoBehaviour
{  
    [System.Serializable]
    private struct Wave
    {
        public Unit[] UnitsToSpawn;
        public int CurrencyBonus;
        public int TurnToStartWave;
    }

    public static event Action OnWaveStarted;
    public static event Action OnWaveCompleted;
    public static event Action<Unit> OnWaveUnitSpawn;
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float showUnitsTime = 1f;
    private CurrencyBank currencyBank;
    private GridManager gridManager;
    private List<Unit> unitsSpawnedThisWave = new List<Unit>();

    private void Start() 
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        currencyBank = FindObjectOfType<CurrencyBank>();
        gridManager = FindObjectOfType<GridManager>();
        StartWave(0);
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        if(eventArgs.IsPlayersTurn)
        {
            StartWave(eventArgs.Turn);
        }
    }

    private void StartWave(int turn)
    {
        foreach (Wave wave in waves)
        {
            if(wave.TurnToStartWave == turn)
            {
                currencyBank.AddCurrencyToBank(wave.CurrencyBonus);
                SpawnUnits(wave);
                StartCoroutine(ShowSpawnedUnits());
                Debug.Log($"Wave spawning...");
            }
        }
    }

    private void SpawnUnits(Wave wave)
    {
        // Create a list of valid spawn points
        int radius = wave.UnitsToSpawn.Length / 3;
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (Transform point in spawnPoints)
        {
            spawnPositions = spawnPositions.Concat(GetValidSpawnGridPositionsForSpawnPoint(point.position, radius)).ToList();;
        }

        // Spawn the units in random locations near the spawn points.
        unitsSpawnedThisWave.Clear();
        foreach (Unit unit in wave.UnitsToSpawn)
        {
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unit, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                unitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }
    }

    private List<Vector2Int> GetValidSpawnGridPositionsForSpawnPoint(Vector3 spawnPoint,  int radius)
    {
        List<Vector2Int> spawnPositions = new List<Vector2Int>();

        Vector2Int spawnPosition = gridManager.GetGridPositionFromWorldPosition(spawnPoint);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int testPosition = new Vector2Int(spawnPosition.x + x, spawnPosition.y + y);
                if(gridManager.IsValidGridPosition(testPosition)
                    && gridManager.GetGridObject(testPosition).IsWalkable(false))
                {
                    spawnPositions.Add(testPosition);
                }
            }
        }

        return spawnPositions;
    }

    private IEnumerator ShowSpawnedUnits()
    {
        OnWaveStarted?.Invoke();

        unitsSpawnedThisWave.Sort(delegate(Unit unitA, Unit unitB)
        {
            if(unitA.transform.position.y > unitB.transform.position.y + 4)
            {
                if(unitA.transform.position.x > unitB.transform.position.x)
                {
                    return -3;
                }
                else if(unitA.transform.position.x < unitB.transform.position.x)
                {
                    return -2;
                }
                return -1;
            }
            else if(unitA.transform.position.y < unitB.transform.position.y - 4)
            {
                if(unitA.transform.position.x > unitB.transform.position.x)
                {
                    return 1;
                }
                else if(unitA.transform.position.x < unitB.transform.position.x)
                {
                    return 2;
                }
                return 3;
            }
            return 0;
        });
        
        // Hide all the units.
        foreach (Unit unit in unitsSpawnedThisWave)
        {
            unit.GetUnitAnimator().HideUnit();
        }

        // Show the units one at a time.
        foreach (Unit unit in unitsSpawnedThisWave)
        {
            OnWaveUnitSpawn?.Invoke(unit);
            yield return StartCoroutine(unit.GetUnitAnimator().SpawnAnimationRoutine(showUnitsTime));
        }
        OnWaveCompleted?.Invoke();
    }
}
