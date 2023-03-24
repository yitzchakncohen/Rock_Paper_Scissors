using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Rock_Paper_Scissors/UnitStats", order = 0)]
public class UnitStats : ScriptableObject 
{   
    public UnitClass unitClass;
    public Sprite[] unitSprites;
    public int moveDistance;
    public int attackDamage;
    public int attackRange;
    public int defense;
    public int unitCost;
    public int maximumHealth;
}
