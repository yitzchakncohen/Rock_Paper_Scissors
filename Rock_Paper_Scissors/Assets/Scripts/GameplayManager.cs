using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

/// <summary>
/// <c>GameplayManager</c> manages the game flow, including identifying end game states. 
/// </summary>
public class GameplayManager : MonoBehaviour
{
    public static event Action OnGameOver;
    private void Awake() 
    {
        UnitHealth.OnDeath += UnitHealth_OnDeath;
    }
    private void OnDestroy() 
    {
        UnitHealth.OnDeath -= UnitHealth_OnDeath;
    }

    private void UnitHealth_OnDeath(object sender, Unit e)
    {
        if(e.GetUnitClass() == UnitClass.PillowFort)
        {
            OnGameOver?.Invoke();
        }
    }

}
