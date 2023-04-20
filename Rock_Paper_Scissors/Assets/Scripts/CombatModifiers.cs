using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Unit;
using UnityEngine;

public class CombatModifiers
{
    public static int GetDamage(Unit attackingUnit, Unit defendingUnit)
    {
        int damage = attackingUnit.GetModifiedAttack();

        damage = damage - defendingUnit.GetModifiedDefense();

        damage = (int)(damage * GetModiferByClasses(attackingUnit.GetUnitClass(), defendingUnit.GetUnitClass()));

        return damage;
    }

    public static int UnitHasAdvantage(UnitClass attackingUnit, UnitClass defendingUnit)
    {
        int advantage = 1;
        int neutral = 0;
        int disadvantage = -1;

        if(attackingUnit == UnitClass.Rock)
        {
            if(defendingUnit == UnitClass.Scissors)
            {
                return advantage;
            }
            else if(defendingUnit == UnitClass.Paper)
            {
                return disadvantage;
            }
            return neutral;           
        }
        else if(attackingUnit == UnitClass.Paper)
        {
            if(defendingUnit == UnitClass.Rock)
            {
                return advantage;
            }
            else if(defendingUnit == UnitClass.Scissors)
            {
                return disadvantage;
            }
            return neutral;         
        }
        else if(attackingUnit == UnitClass.Scissors)
        {
            if(defendingUnit == UnitClass.Paper)
            {
                return advantage;
            }
            else if(defendingUnit == UnitClass.Rock)
            {
                return disadvantage;
            }
            return neutral;         
        }
        return neutral;
    }

    private static float GetModiferByClasses(UnitClass attackingUnit, UnitClass defendingUnit)
    {
        float positiveClassModifier = 1.5f;
        float negativeClassModifier = 0.75f;
        
        switch (UnitHasAdvantage(attackingUnit, defendingUnit))
        {
            case -1:
                return negativeClassModifier;
            case 1: 
                return positiveClassModifier;
            case 0:
            default:
                return 1f;
        }
    }
}
