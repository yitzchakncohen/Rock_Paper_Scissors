using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

public class WaveManager : MonoBehaviour, ISaveInterface<SaveWaveManagerData>
{  
    [System.Serializable]
    private struct Wave
    {
        public Unit[] EnemyUnitTypesToSpawn;
        public Unit[] FriendlyUnitTypesToSpawn;
        public int TotalEnemyUnitsToSpawn;
        public int TotalFriendlyUnitsToSpawn;
        public int CurrencyBonus;
        public int TurnToStartWave;
    }

    public static event Action OnWaveStarted;
    public static event Action OnWaveCompleted;
    public static event Action<Unit> OnWaveUnitSpawn;
    public static event Action<int> OnTurnsUntilNextWaveUpdated;
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private Transform friendlySpawnPoint;
    [SerializeField] private float showUnitsTime = 1f;
    [SerializeField] private Unit homeBasePrefab; 
    private CurrencyBank currencyBank;
    private GridManager gridManager;
    private int turnsUntilNextWave = 0;
    private int minimumTurnsBetweenWaves = 4;
    private bool startWaveWhenReady = false;

    private void Start() 
    {
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;
        currencyBank = FindObjectOfType<CurrencyBank>();
        gridManager = FindObjectOfType<GridManager>();
        UpdateTurnsUntilNextWave(0);
    }

    private void Update() 
    {
        if(startWaveWhenReady)
        {
            if(gridManager.SetupGridTask.IsCompleted)
            {
                startWaveWhenReady = false;
                TryStartWave(0);
            }
        }
    }

    [ContextMenu("Start Wave 0")]
    public void StartWaveZero()
    {
        TryStartWave(0);
    }

    public void StartWaveWhenReady()
    {
        startWaveWhenReady = true;
    }

    private void OnDestroy() 
    {
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs eventArgs)
    {
        if (eventArgs.IsPlayersTurn)
        {
            TryStartWave(eventArgs.Turn);
        }

        UpdateTurnsUntilNextWave(eventArgs.Turn);
    }

    private void UpdateTurnsUntilNextWave(int currentTurn)
    {
        int nextWaveTurn = currentTurn;
        if(currentTurn <= waves[waves.Length-1].TurnToStartWave )
        {
            foreach (Wave wave in waves)
            {
                if (wave.TurnToStartWave > currentTurn)
                {
                    nextWaveTurn = wave.TurnToStartWave;
                    break;
                }
            }
            turnsUntilNextWave = nextWaveTurn - currentTurn;
        }
        OnTurnsUntilNextWaveUpdated.Invoke(turnsUntilNextWave);
    }

    private void TryStartWave(int turn)
    {
        if(turn > waves[waves.Length-1].TurnToStartWave)
        {
            turnsUntilNextWave--;
            // Ran out of waves
            if(turnsUntilNextWave == 0)
            {
                // Use the last wave
                Wave wave = waves[waves.Length-1];
                StartWave(turn, wave);
                turnsUntilNextWave = minimumTurnsBetweenWaves;
                OnTurnsUntilNextWaveUpdated.Invoke(turnsUntilNextWave);
            }
            return;
        }
        else
        {
            foreach (Wave wave in waves)
            {
                if(wave.TurnToStartWave == turn)
                {
                    StartWave(turn, wave);
                }
            }
        }

    }

    private void StartWave(int turn, Wave wave)
    {
        currencyBank.AddCurrencyToBank(wave.CurrencyBonus, null);

        List<Unit> unitsSpawnedThisWave = SpawnEnemyUnits(wave.EnemyUnitTypesToSpawn, wave.TotalEnemyUnitsToSpawn);
        unitsSpawnedThisWave.AddRange(SpawnFriendlyUnits(wave.FriendlyUnitTypesToSpawn, wave.TotalFriendlyUnitsToSpawn, turn));
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
        int radius = unitTypesToSpawn.Length / 3;
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        foreach (Transform point in enemySpawnPoints)
        {
            spawnPositions = spawnPositions.Concat(GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), point.position, radius)).ToList();
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

    private List<Unit> SpawnFriendlyUnits(Unit[] unitTypesToSpawn, int totalUnitsToSpawn, int turn)
    {

        List<Unit> friendlyUnitsSpawnedThisWave = new List<Unit>();
        int radius = unitTypesToSpawn.Length / 3;
        
        if(turn == 0)
        {
            //Spawn the home base in the middle on the first turn. 
            Vector2Int spawnPosition = gridManager.GetGridPositionFromWorldPosition(friendlySpawnPoint.position);
            Unit spawnedUnit = Instantiate(homeBasePrefab, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
            friendlyUnitsSpawnedThisWave.Add(spawnedUnit);
        }        

        if(unitTypesToSpawn.Length == 0 )
        {
            return friendlyUnitsSpawnedThisWave;
        }
        List<Vector2Int> spawnPositions = GetValidSpawnGridPositionsForSpawnPoint(unitTypesToSpawn.FirstOrDefault(), friendlySpawnPoint.position, radius);
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

    private List<Vector2Int> GetValidSpawnGridPositionsForSpawnPoint(IGridOccupantInterface gridObject, Vector3 spawnPoint,  int radius)
    {
        List<Vector2Int> spawnPositions = new List<Vector2Int>();

        Vector2Int spawnPosition = gridManager.GetGridPositionFromWorldPosition(spawnPoint);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int testPosition = new Vector2Int(spawnPosition.x + x, spawnPosition.y + y);
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
