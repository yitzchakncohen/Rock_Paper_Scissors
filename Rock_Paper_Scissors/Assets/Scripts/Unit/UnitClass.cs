using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public enum UnitClass
    {
        Rock = 1 << 1,
        Paper = 1 << 2,
        Scissors = 1 << 3,
        PillowFort = 1 << 4,
        PillowOutpost = 1 << 5,
        GlueTrap = 1 << 6,
        TrampolineTrap = 1 << 7,
        Moveable = Rock | Paper | Scissors,
        Building = PillowFort | PillowOutpost,
        Trap = GlueTrap | TrampolineTrap
    }
}
