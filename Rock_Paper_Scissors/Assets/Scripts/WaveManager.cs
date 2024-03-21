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
        public Unit[] EnemyUnitsToSpawn;
        public Unit[] FriendlyUnitsToSpawn;
        public int CurrencyBonus;
        public int TurnToStartWave;
    }

    public static event Action OnWaveStarted;
    public static event Action OnWaveCompleted;
    public static event Action<Unit> OnWaveUnitSpawn;
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private Transform friendlySpawnPoint;
    [SerializeField] private float showUnitsTime = 1f;
    [SerializeField] private Unit homeBasePrefab; 
    private CurrencyBank currencyBank;
    private GridManager gridManager;

    private void Start() 
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        currencyBank = FindObjectOfType<CurrencyBank>();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        if(eventArgs.IsPlayersTurn)
        {
            StartWave(eventArgs.Turn);
        }
    }

    public void StartWave(int turn)
    {
        foreach (Wave wave in waves)
        {
            if(wave.TurnToStartWave == turn)
            {
                currencyBank.AddCurrencyToBank(wave.CurrencyBonus);

                List<Unit> unitsSpawnedThisWave = SpawnEnemyUnits(wave.EnemyUnitsToSpawn);
                unitsSpawnedThisWave.AddRange(SpawnFriendlyUnits(wave.FriendlyUnitsToSpawn, turn));
                gridManager.UpdateGridOccupancy();

                StartCoroutine(ShowSpawnedUnits(unitsSpawnedThisWave));
                Debug.Log($"Wave spawning...");
            }
        }
    }

    private List<Unit> SpawnEnemyUnits(Unit[] unitsToSpawn)
    {
        List<Unit> enemyUnitsSpawnedThisWave = new List<Unit>();
        // Create a list of valid spawn points
        int radius = unitsToSpawn.Length / 3;
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (Transform point in enemySpawnPoints)
        {
            spawnPositions = spawnPositions.Concat(GetValidSpawnGridPositionsForSpawnPoint(point.position, radius)).ToList();
        }

        // Spawn the units in random locations near the spawn points.
        foreach (Unit unit in unitsToSpawn)
        {
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unit, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                enemyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }
        return enemyUnitsSpawnedThisWave;
    }

    private List<Unit> SpawnFriendlyUnits(Unit[] unitsToSpawn, int turn)
    {
        List<Unit> friendlyUnitsSpawnedThisWave = new List<Unit>();
        int radius = unitsToSpawn.Length / 3;
        
        if(turn == 0)
        {
            //Spawn the home base in the middle on the first turn. 
            Vector2Int spawnPosition = gridManager.GetGridPositionFromWorldPosition(friendlySpawnPoint.position);
            Unit spawnedUnit = Instantiate(homeBasePrefab, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
            friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
        }        

        List<Vector2Int> spawnPositions = GetValidSpawnGridPositionsForSpawnPoint(friendlySpawnPoint.position, radius);
        foreach (Unit unit in unitsToSpawn)
        {
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unit, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }

        return friendlyUnitsSpawnedThisWave;
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

    private IEnumerator ShowSpawnedUnits(List<Unit> unitsSpawnedThisWave)
    {
        OnWaveStarted?.Invoke();

        unitsSpawnedThisWave.Sort(delegate(Unit unitA, Unit unitB)
        {
            int friendlyBonus = 0; 
            if(unitA.IsFriendly())
            {
                friendlyBonus = 100;
            }

            if(unitA.transform.position.y > unitB.transform.position.y + 4)
            {
                if(unitA.transform.position.x > unitB.transform.position.x)
                {
                    return -3 + friendlyBonus;
                }
                else if(unitA.transform.position.x < unitB.transform.position.x)
                {
                    return -2 + friendlyBonus;
                }
                return -1 + friendlyBonus;
            }
            else if(unitA.transform.position.y < unitB.transform.position.y - 4)
            {
                if(unitA.transform.position.x > unitB.transform.position.x)
                {
                    return 1 + friendlyBonus;
                }
                else if(unitA.transform.position.x < unitB.transform.position.x)
                {
                    return 2 + friendlyBonus;
                }
                return 3 + friendlyBonus;
            }
            return 0 + friendlyBonus;
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
