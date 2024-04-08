using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

public class GlueTrap : UnitTrap
{
    protected override void AnimateTrap()
    {
        // Trap animation
        trapBackground.SetActive(true);
        trapForeground.SetActive(true);
    }

    protected override void ApplyTrapEffect(Unit trappedUnit)
    {
        // Damage unit
        int damageAmount = CombatModifiers.GetDamage(unit, trappedUnit, false);
        trappedUnit.Damage(damageAmount, unit);
    }
}
