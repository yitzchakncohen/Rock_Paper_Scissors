using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

public struct SaveUnitData
{
    public UnitClass UnitClass;
    public int Level;
    public int ActionPointsRemaining; 
    public int UnitHealth;
    public Vector2Int GridPosition;
    public bool IsFriendly;
    public Direction FacingDirection;
}

public enum Direction
{
    North,
    South,
    East,
    West
}
