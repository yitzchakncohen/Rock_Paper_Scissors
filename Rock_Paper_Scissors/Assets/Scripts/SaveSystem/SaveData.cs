using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

[System.Serializable]
public struct SaveData
{
    public List<SaveUnitData> UnitList;
    public SaveCurrencyBankData SaveFriendlyCurrencyBankData;
    public SaveCurrencyBankData SaveEnemyCurrencyBankData;
    public SaveTurnManagerData SaveTurnManagerData;
    public SaveGameplayManagerData SaveGameplayManagerData;
    public SaveWaveManagerData SaveWaveManagerData;
}
