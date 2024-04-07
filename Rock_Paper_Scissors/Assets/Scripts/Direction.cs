using System;

[System.Serializable]
[Flags]
public enum Direction
{
    None = 0,
    NorthWest = 1,
    NorthEast = 1 << 1,
    SouthWest = 1 << 2,
    SouthEast = 1 << 3,
    West = 1 << 4,
    East = 1 << 5,

    AllWest = NorthWest | West | SouthWest,
    AllEast = NorthEast | East | SouthEast,
    AllNorth = NorthWest | NorthEast,
    AllSouth = SouthWest | SouthEast,
    NorthAndEast = East | AllNorth,
    NorthAndWest = West | AllNorth,
    SouthAndEast = East | AllSouth,
    SouthAndWest = West | AllSouth,
    WestNorthWest = West | NorthWest,
    WestSouthWest = West | SouthWest,
    EastNorthEast = East | NorthEast,
    EastSouthEast = East | SouthEast,
}