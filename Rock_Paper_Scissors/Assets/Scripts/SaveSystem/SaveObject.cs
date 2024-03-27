using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

[System.Serializable]
struct SaveObject
{
    public List<SaveUnitData> UnitList;
    public SaveCurrencyBankData SaveCurrencyBankData;
    public SaveTurnManagerData SaveTurnManagerData;
    public SaveGameplayManagerData SaveGameManagerData;
}
