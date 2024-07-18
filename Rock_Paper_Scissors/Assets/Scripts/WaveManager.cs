using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissors;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

public class WaveManager : MonoBehaviour, ISaveInterface<SaveWaveManagerData>
{  
    public static event Action OnWaveStarted;
    public static event Action OnWaveCompleted;
    public static event Action<Unit> OnWaveUnitSpawn;
    public static event Action<int> OnTurnsUntilNextWaveUpdated;
    [SerializeField] private Wave[] endlessModeWaves;
    [SerializeField] private float showUnitsTime = 1f;
    [SerializeField] private Unit homeBasePrefab; 
    private CurrencyBank currencyBank;
    private GridManager gridManager;
    private int turnsUntilNextWave = 0;
    private int minimumTurnsBetweenWaves = 4;
    private GameMode gameMode;

    private void Start() 
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        currencyBank = FindObjectOfType<CurrencyBank>();
        gridManager = FindObjectOfType<GridManager>();
        UpdateTurnsUntilNextWave(1);
    }

    public void StartWaveWhenReady(Wave wave, GameMode gameMode)
    {
        this.gameMode = gameMode;
        StartWhenReadyAsync(wave);
    }

    public async void StartWhenReadyAsync(Wave wave)
    {
        await gridManager.SetupGridTask;
        if(gameMode == GameMode.Endless)
        {
            StartWave(1, endlessModeWaves[0]);
        }
        else
        {
            StartWave(1, wave);
        }
    }

    private void OnDestroy() 
    {
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        if (eventArgs.IsPlayersTurn)
        {
            if(gameMode == GameMode.Endless)
            {
                UpdateTurnsUntilNextWave(eventArgs.Turn);
                TryStartWave(eventArgs.Turn);
            }
        }
    }

    private void UpdateTurnsUntilNextWave(int currentTurn)
    {
        if(gameMode == GameMode.Endless)
        {
            OnTurnsUntilNextWaveUpdated.Invoke(0);
            return;
        }

        int nextWaveTurn = currentTurn;
        if(currentTurn <= endlessModeWaves[endlessModeWaves.Length-1].TurnToStartWave )
        {
            foreach (Wave wave in endlessModeWaves)
            {
                if (wave.TurnToStartWave >= currentTurn)
                {
                    nextWaveTurn = wave.TurnToStartWave;
                    break;
                }
            }
            turnsUntilNextWave = nextWaveTurn - currentTurn;
        }
        else
        {
            if(currentTurn == endlessModeWaves[endlessModeWaves.Length-1].TurnToStartWave + 1)
            {
                // Handle the transition from set waves to procedural waves.
                turnsUntilNextWave = minimumTurnsBetweenWaves;
            }
            else
            {
                turnsUntilNextWave--;
                if(turnsUntilNextWave < 0)
                {
                    turnsUntilNextWave = minimumTurnsBetweenWaves;
                }
            }
        }
        OnTurnsUntilNextWaveUpdated.Invoke(turnsUntilNextWave);
    }

    private void TryStartWave(int turn)
    {
        if(turnsUntilNextWave == 0)
        {
            if(turn <= endlessModeWaves[endlessModeWaves.Length-1].TurnToStartWave)
            {
                foreach (Wave wave in endlessModeWaves)
                {
                    if(wave.TurnToStartWave == turn)
                    {
                        StartWave(turn, wave);
                    }
                }
            }
            else
            {
                // Use the last wave
                Wave wave = endlessModeWaves[endlessModeWaves.Length-1];
                StartWave(turn, wave);
            }
        }
    }

    private void StartWave(int turn, Wave wave)
    {
        currencyBank.AddCurrencyToBank(wave.CurrencyBonus, null);

        List<Unit> unitsSpawnedThisWave = SpawnEnemyBuildings(wave.EnemyBuildingsToSpawn);
        gridManager.UpdateGridOccupancy();
        unitsSpawnedThisWave.AddRange(SpawnEnemyUnits(wave.EnemyUnitTypesToSpawn, wave.TotalEnemyUnitsToSpawn));
        gridManager.UpdateGridOccupancy();
        unitsSpawnedThisWave.AddRange(SpawnFriendlyBuildings(wave.FriendlyBuildingsToSpawn, turn));
        gridManager.UpdateGridOccupancy();
        unitsSpawnedThisWave.AddRange(SpawnFriendlyUnits(wave.FriendlyUnitTypesToSpawn, turn));
        gridManager.UpdateGridOccupancy();

        StartCoroutine(ShowSpawnedUnits(unitsSpawnedThisWave));
        Debug.Log($"Wave spawning...");
    }

    private List<Unit> SpawnEnemyUnits(Unit[] unitTypesToSpawn, int totalUnitsToSpawn)
    {
        if(unitTypesToSpawn.Length == 0 )
        {
            return new List<Unit>();
        }

        List<Unit> enemyUnitsSpawnedThisWave = new List<Unit>();
        // Create a list of valid spawn points
        int radius = Mathf.Max(unitTypesToSpawn.Length / gridManager.SpawnPoints.Count, 1);
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (Vector2Int point in gridManager.SpawnPoints)
        {
            spawnPositions = spawnPositions.Concat(GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), point, radius)).ToList();
        }

        // Spawn the units in random locations near the spawn points.
        for (int i = 0; i < totalUnitsToSpawn; i++)
        {
            int unitToSpawnIndex = UnityEngine.Random.Range(0, unitTypesToSpawn.Length);
            Unit unitToSpawn = unitTypesToSpawn[unitToSpawnIndex];
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unitToSpawn, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                enemyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }

        return enemyUnitsSpawnedThisWave;
    }

    private List<Unit> SpawnEnemyBuildings(Unit[] unitTypesToSpawn)
    {
        if(unitTypesToSpawn.Length == 0 )
        {
            return new List<Unit>();
        }

        List<Unit> enemyUnitsSpawnedThisWave = new List<Unit>();
        // Create a list of valid spawn points
        int radius = unitTypesToSpawn.Length / 3;
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (Vector2Int point in gridManager.SpawnPoints)
        {
            spawnPositions = spawnPositions.Concat(GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), point, radius)).ToList();
        }

        // Spawn the units in random locations near the spawn points.
        for (int i = 0; i < unitTypesToSpawn.Length; i++)
        {
            Unit unitToSpawn = unitTypesToSpawn[i];
            if(spawnPositions.Count() > 0)
            {
                Vector2Int spawnPosition;
                if(i == 0 && spawnPositions.Contains(gridManager.SpawnPoints.First()))
                {
                    spawnPosition = gridManager.SpawnPoints.First();
                }
                else 
                {
                    int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                    spawnPosition = spawnPositions[spawnPositionIndex];
                }
                Unit spawnedUnit = Instantiate(unitToSpawn, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                enemyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }

        return enemyUnitsSpawnedThisWave;
    }

    private List<Unit> SpawnFriendlyBuildings(Unit[] unitTypesToSpawn, int turn)
    {

        List<Unit> friendlyUnitsSpawnedThisWave = new List<Unit>();
        int radius = 3;
        
        if(turn == 1)
        {
            //Spawn the home base in the middle on the first turn. 
            Unit spawnedUnit = Instantiate(homeBasePrefab, gridManager.GetGridObject(gridManager.PlayerStartingPoint).transform.position, Quaternion.identity);
            friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
            gridManager.UpdateGridOccupancy();
        } 

        if(unitTypesToSpawn.Length == 0)
        {
            return friendlyUnitsSpawnedThisWave;
        }
        List<Vector2Int> spawnPositions = GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), gridManager.PlayerStartingPoint, radius);
        for (int i = 0; i < unitTypesToSpawn.Length; i++)
        {
            Unit unitToSpawn = unitTypesToSpawn[i];
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unitToSpawn, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }

        return friendlyUnitsSpawnedThisWave;
    }

    private List<Unit> SpawnFriendlyUnits(Unit[] unitTypesToSpawn, int totalUnitsToSpawn)
    {

        List<Unit> friendlyUnitsSpawnedThisWave = new List<Unit>();
        int radius = 3;

        if(unitTypesToSpawn.Length == 0)
        {
            return friendlyUnitsSpawnedThisWave;
        }
        List<Vector2Int> spawnPositions = GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), gridManager.PlayerStartingPoint, radius);
        for (int i = 0; i < totalUnitsToSpawn; i++)
        {
            int unitToSpawnIndex = UnityEngine.Random.Range(0, unitTypesToSpawn.Length);
            Unit unitToSpawn = unitTypesToSpawn[unitToSpawnIndex];
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Unit spawnedUnit = Instantiate(unitToSpawn, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
                friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
                spawnPositions.Remove(spawnPosition);
            }
        }

        return friendlyUnitsSpawnedThisWave;
    }

    private List<Vector2Int> GetValidSpawnGridPositionsForSpawnPoint(IGridOccupantInterface gridObject, Vector2Int spawnPoint,  int radius)
    {
        List<Vector2Int> spawnPositions = new List<Vector2Int>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int testPosition = new Vector2Int(spawnPoint.x + x, spawnPoint.y + y);
                if(gridManager.IsValidGridPosition(testPosition)
                    && gridManager.GetGridObject(testPosition).IsWalkable(gridObject))
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
        AudioManager.Instance.PlayEnemyWaveSound();

        unitsSpawnedThisWave.Sort(delegate(Unit unitA, Unit unitB)
        {
            int friendlyBonus = 0; 
            if(unitA.IsFriendly)
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
            unit.UnitAnimator.HideUnit();
        }

        // Show the units one at a time.
        foreach (Unit unit in unitsSpawnedThisWave)
        {
            AudioManager.Instance.PlayUnitSpawnSound();
            OnWaveUnitSpawn?.Invoke(unit);
            yield return StartCoroutine(unit.UnitAnimator.SpawnAnimationRoutine(showUnitsTime));
        }
        OnWaveCompleted?.Invoke();
    }

    public SaveWaveManagerData Save()
    {
        return new SaveWaveManagerData
        {
            TurnsUntilNextWave = turnsUntilNextWave
        };
    }

    public void Load(SaveWaveManagerData loadData)
    {
        turnsUntilNextWave = loadData.TurnsUntilNextWave;
        OnTurnsUntilNextWaveUpdated.Invoke(turnsUntilNextWave);
    }
}
