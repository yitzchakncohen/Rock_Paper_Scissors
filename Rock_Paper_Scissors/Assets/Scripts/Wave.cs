using RockPaperScissors.Units;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Rock_Paper_Scissors/Wave", order = 0)]
public class Wave : ScriptableObject 
{
    public Unit[] EnemyUnitTypesToSpawn;
    public Unit[] FriendlyUnitTypesToSpawn;
    public int TotalEnemyUnitsToSpawn;
    public int TotalFriendlyUnitsToSpawn;
    public int CurrencyBonus;
    public int TurnToStartWave;
}
