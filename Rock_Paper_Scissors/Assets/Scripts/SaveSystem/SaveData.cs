using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

[System.Serializable]
public struct SaveData
{
    public GameMode GameMode;
    public int Level;
    public List<SaveUnitData> UnitList;
    public SaveCurrencyBankData SaveCurrencyBankData;
    public SaveTurnManagerData SaveTurnManagerData;
    public SaveGameplayManagerData SaveGameplayManagerData;
    public SaveWaveManagerData SaveWaveManagerData;
}
