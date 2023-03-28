using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    private CurrencyBank currencyBank;
    private GridManager gridManager;

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
        foreach (Unit unit in wave.UnitsToSpawn)
        {
            if(spawnPositions.Count() > 0)
            {
                int spawnPositionIndex = UnityEngine.Random.Range(0, spawnPositions.Count());
                Vector2Int spawnPosition = spawnPositions[spawnPositionIndex];
                Instantiate(unit, gridManager.GetGridObject(spawnPosition).transform.position, Quaternion.identity);
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
                    && gridManager.GetGridObject(testPosition).IsWalkable())
                {
                    spawnPositions.Add(testPosition);
                }
            }
        }

        return spawnPositions;
    }
}
