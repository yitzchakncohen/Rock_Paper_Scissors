using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

public class CombatModifiers
{
    public static int GetDamage(Unit attackingUnit, Unit defendingUnit, bool defendingUnitInTower)
    {
        int damage = attackingUnit.AttackDamage;

        damage = damage - defendingUnit.Defense * GetInTowerModifier(defendingUnitInTower);

        damage = (int)(damage * GetModiferByClasses(attackingUnit.Class, defendingUnit.Class));

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
        const float POSITIVE_CLASS_MODIFIER = 2.0f;
        const float NEGATIVE_CLASS_MODIFIER = 0.5f;
        
        switch (UnitHasAdvantage(attackingUnit, defendingUnit))
        {
            case -1:
                return NEGATIVE_CLASS_MODIFIER;
            case 1: 
                return POSITIVE_CLASS_MODIFIER;
            case 0:
            default:
                return 1f;
        }
    }

    private static int GetInTowerModifier(bool defendingUnitInTower)
    {
        int inTowerDefenseModifier = 2;
        if(defendingUnitInTower)
        {
            return inTowerDefenseModifier;
        }
        else
        {
            return 1;
        }
    }
}
