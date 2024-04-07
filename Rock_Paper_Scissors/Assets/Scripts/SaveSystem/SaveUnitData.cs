using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.SaveSystem
{
    [System.Serializable]
    public struct SaveUnitData
    {
        // GridPosition for spawning purposes
        public Vector2Int GridPosition;
        // Spawn unit of correct class
        public UnitClass UnitClass;
        public int UnitLevel;
        public int AttackActionPointsRemaining; 
        public int MoveActionPointsRemaining;
        public int SpawnerActionPointsRemaining;
        public int TrappedTurnsRemaining;
        public int UnitHealth;
        public int UnitXP;
        public bool TrapIsSprung;
        public bool IsFriendly;
        public Direction FacingDirection;
    }
}

[System.Serializable]
public enum Direction
{
    NorthWest,
    NorthEast,
    SouthWest,
    SouthEast,
    East,
    West
}
