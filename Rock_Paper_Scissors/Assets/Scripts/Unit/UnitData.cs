using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Rock_Paper_Scissors/UnitStats", order = 0)]
public class UnitData : ScriptableObject 
{   
    public UnitClass unitClass;
    public Sprite unitThumbnail;
    public SpriteLibraryAsset spriteLibrary;
    public int moveDistance;
    public int[] attackDamage;
    public int attackRange;
    public int[] defense;
    public int unitCost;
    public int[] maximumHealth;
}
