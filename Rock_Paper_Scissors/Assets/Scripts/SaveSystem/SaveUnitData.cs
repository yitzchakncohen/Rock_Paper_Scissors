using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    public struct SaveUnitData
    {
        public UnitData UnitData;
        public int UnitLevel;
        public int AttackActionPointsRemaining; 
        public int MoveActionPointsRemaining; 
        public int UnitHealth;
        public int UnitXP;
        public Vector2Int GridPosition;
        public bool IsFriendly;
        public Direction FacingDirection;
    }
}

public enum Direction
{
    NorthWest,
    NorthEast,
    SouthWest,
    SouthEast,
    East,
    West
}
