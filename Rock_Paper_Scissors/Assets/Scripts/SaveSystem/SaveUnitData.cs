using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    public struct SaveUnitData
    {
        // GridPosition for spawning purposes
        public Vector2Int GridPosition;
        public UnitData UnitData;
        public int UnitLevel;
        public int AttackActionPointsRemaining; 
        public int MoveActionPointsRemaining; 
        public int UnitHealth;
        public int UnitXP;
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
