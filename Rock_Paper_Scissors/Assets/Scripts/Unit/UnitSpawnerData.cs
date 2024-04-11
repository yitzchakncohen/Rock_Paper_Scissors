using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.Units
{

    [CreateAssetMenu(fileName = "UnitSpawnerData", menuName = "Rock_Paper_Scissors/UnitSpawnerData", order = 0)]
    public class UnitSpawnerData : ScriptableObject 
    {
        public List<Unit> SpawnableUnits;
        public int[] SpawnRadius;
        public int[] CurrencyProducedPerTurn;
        public int ActionPoints = 2;
        public int[] UpgradeCost;
    }
}
