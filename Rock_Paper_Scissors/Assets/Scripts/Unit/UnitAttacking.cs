using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttacking : MonoBehaviour
{
    public static event EventHandler OnAttackingCompleted;
    [SerializeField] private int attackRange = 1;
    [SerializeField] private int attackDamage = 10;
    private Unit unit;
    private GridManager gridManager;
    private int attackPointsRemaining = 1;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
    }

    private void Start() 
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    public bool TryAttackUnit(Unit unitToAttack)
    {
        if(attackPointsRemaining <= 0)
        {
            return false;
        }

        if(GetValidTargets().Contains(unitToAttack))
        {
            unitToAttack.Damage(attackDamage);
            OnAttackingCompleted?.Invoke(this, EventArgs.Empty);
            attackPointsRemaining -= 1;
            return true;
        }
        return false;
    }

    public List<Unit> GetValidTargets()
    {
        List<Unit> validTargetList = new List<Unit>();

        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);

        // Check all the targets in range.
        // Get list of grid positions in range.
        List<Vector2Int> gridPositionsInRangeList = new List<Vector2Int>();
        gridPositionsInRangeList.Add(gridPosition);

        List<Vector2Int> newPositions = new List<Vector2Int>();

        for (int i = 0; i < attackRange; i++)
        {
            newPositions.Clear();
            foreach (Vector2Int position in gridPositionsInRangeList)
            {
                foreach (Vector2Int neighbourPosition in GetNeighbourList(position))
                {
                    if(!newPositions.Contains(neighbourPosition))
                    {
                        newPositions.Add(neighbourPosition);
                    }
                }
            }

            foreach (Vector2Int newPosition in newPositions)
            {
                if(!gridPositionsInRangeList.Contains(newPosition))
                {
                    gridPositionsInRangeList.Add(newPosition);
                }
            }
        }

        // Check grid positions for valid targets
        foreach (Vector2Int position in gridPositionsInRangeList)
        {
            GridObject gridObject = gridManager.GetGridObject(position);
            if(gridObject.GetOccupent() != null && gridObject.GetOccupent().IsFriendly() != unit.IsFriendly())
            {
                validTargetList.Add(gridObject.GetOccupent());
            }
        }

        return validTargetList;
    }

    private List<Vector2Int> GetNeighbourList(Vector2Int currentPosition)
    {
        List<Vector2Int> neighbourList = new List<Vector2Int>();

        bool oddRow = currentPosition.y % 2 == 1;

        if(currentPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(new Vector2Int(currentPosition.x -1, currentPosition.y +0));

        }

        if(currentPosition.x + 1 < gridManager.GetGridSize().x)
        {
            // Right
            neighbourList.Add(new Vector2Int(currentPosition.x +1, currentPosition.y +0));
        }

        if(currentPosition.y -1 >= 0)
        {
            // Down
            neighbourList.Add(new Vector2Int(currentPosition.x +0, currentPosition.y -1));
            if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < gridManager.GetGridSize().x)
            {
                neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y -1));                
            }
        }

        if(currentPosition.y + 1 < gridManager.GetGridSize().y)
        {
            // Up
            neighbourList.Add(new Vector2Int(currentPosition.x + 0, currentPosition.y +1));
            if(currentPosition.x - 1 >= 0 && currentPosition.x + 1 < gridManager.GetGridSize().x)
            {
                neighbourList.Add(new Vector2Int(currentPosition.x + (oddRow ? +1 : -1), currentPosition.y +1));
            }
        }

        return neighbourList;
    }

    public int GetAttackPointsRemaining()
    {
        return attackPointsRemaining;
    }

    public void ResetAttackPoints()
    {
        attackPointsRemaining = 1;
    }
}
