using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack : UnitAction
{
    [SerializeField] private UnitAnimator unitAnimator;
    private Unit unit;
    private Unit target;
    private GridManager gridManager;
    private float timer;
    private bool attacking;
    private int unitAttackActionBaseValue = 100;


    private void Awake() 
    {
        unit = GetComponent<Unit>();
    }

    protected override void Start() 
    {
        base.Start();
        IsCancellableAction = false;
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update() 
    {
        if(!attacking)
        {
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            target.Damage(unit.GetBaseAttack(), unit);
            AnimateAttack(target.transform.position - transform.position);
            actionPointsRemaining -= 1;
            attacking = false;
            ActionComplete();
        }
    }

    public bool TryAttackUnit(Unit unitToAttack, Action onActionComplete)
    {
        if(actionPointsRemaining <= 0)
        {
            return false;
        }

        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);
        if(GetValidTargets(gridPosition).Contains(unitToAttack))
        {
            timer = 1f;
            target = unitToAttack;
            attacking = true;
            ActionStart(onActionComplete);
            return true;
        }
        return false;
    }

    public List<Unit> GetValidTargets(Vector2Int gridPosition)
    {
        List<Unit> validTargetList = new List<Unit>();

        // Check all the targets in range.
        // Get list of grid positions in range.
        List<Vector2Int> gridPositionsInRangeList = new List<Vector2Int>();
        gridPositionsInRangeList.Add(gridPosition);

        List<Vector2Int> newPositions = new List<Vector2Int>();

        for (int i = 0; i < unit.GetAttackRange(); i++)
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

    private void AnimateAttack(Vector2 attackDirection)
    {
        if(attackDirection.x > 0 && attackDirection.y > 0)
        {
            unitAnimator.MoveUpRight();
        }
        else if(attackDirection.x > 0 && attackDirection.y < 0)
        {
            unitAnimator.MoveDownRight();
        }
        else if(attackDirection.x > 0 && attackDirection.y == 0)
        {
            unitAnimator.MoveRight();
        }
        if(attackDirection.x < 0 && attackDirection.y > 0)
        {
            unitAnimator.MoveUpLeft();
        }
        else if(attackDirection.x < 0 && attackDirection.y < 0)
        {
            unitAnimator.MoveDownLeft();
        }
        else if(attackDirection.x < 0 && attackDirection.y == 0)
        {
            unitAnimator.MoveLeft();
        }
    }

    public override EnemyAIAction GetBestEnemyAIAction()
    {
        EnemyAIAction bestAction = null;
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(unit.transform.position);

        foreach (Unit unit in GetValidTargets(gridPosition))
        {
            GridObject gridObject = gridManager.GetGridObjectFromWorldPosition(unit.transform.position);
            if(bestAction == null)
            {
                bestAction = new EnemyAIAction()
                {
                    gridObject = gridObject,
                    actionValue = unitAttackActionBaseValue + (1 - unit.GetNormalizedHealth())*unitAttackActionBaseValue,
                    unitAction = this,
                };
            }
            else
            {
                EnemyAIAction testAction = new EnemyAIAction()
                {
                    gridObject = gridObject,
                    actionValue = unitAttackActionBaseValue + (1 - unit.GetNormalizedHealth())*100,
                    unitAction = this,
                }; 

                // Check if this action is better.
                if(testAction.actionValue > bestAction.actionValue)
                {
                    bestAction = testAction;
                }
            }
        }

        return bestAction;
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        return TryAttackUnit(gridObject.GetOccupent(), onActionComplete);
    }

    protected override void CancelButton_OnCancelButtonPress()
    {
        base.CancelButton_OnCancelButtonPress();
    }
}
