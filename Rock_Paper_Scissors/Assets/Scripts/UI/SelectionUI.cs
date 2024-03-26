using System;
using RockPaperScissors.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class SelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        [SerializeField] private TextMeshProUGUI unit;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI attack;
        [SerializeField] private TextMeshProUGUI range;
        [SerializeField] private TextMeshProUGUI defense;
        [SerializeField] private TextMeshProUGUI movement;
        [SerializeField] private Image xpBar;
        private Unit selectedUnit = null;

        
        private void Start() 
        {
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            background.SetActive(false);
        }

        private void OnDestroy() 
        {
            ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
        }

        private void ActionHandler_OnUnitSelected(object sender, Unit unit)
        {
            if(unit != null)
            {
                background.SetActive(true);
                UpdateUnitStats(unit);
                selectedUnit = unit;
                selectedUnit.GetUnitProgression().OnLevelUp += selectedUnit_OnLevelUp;
                selectedUnit.GetUnitProgression().OnGainXP += selectedUnit_OnGainXP;
            }
            else
            {
                background.SetActive(false);
                if(selectedUnit != null)
                {
                    selectedUnit.GetUnitProgression().OnLevelUp -= selectedUnit_OnLevelUp;
                    selectedUnit.GetUnitProgression().OnGainXP -= selectedUnit_OnGainXP;
                    selectedUnit = null;
                }
            }
        }

        private void UpdateUnitStats(Unit unit)
        {
            this.unit.text = unit.GetUnitClass().ToString();
            level.text = $"Level: {unit.GetLevel().ToString()}";
            health.text = $" <sprite=0> {unit.GetHealth().ToString()}/{unit.GetMaximumHealth().ToString()}";
            attack.text = $" <sprite=2> {unit.GetBaseAttack()}";
            range.text = $" <sprite=3> {unit.GetAttackRange()}";
            defense.text = $" <sprite=1> {unit.GetBaseDefense()}";
            movement.text = $" <sprite=4> {unit.GetMoveDistance()}";
            xpBar.fillAmount = unit.GetUnitProgression().GetXP() % 100 / 100.0f;
        }

        private void selectedUnit_OnLevelUp()
        {
            UpdateUnitStats(selectedUnit);
        }

        private void selectedUnit_OnGainXP()
        {
            xpBar.fillAmount = selectedUnit.GetUnitProgression().GetXP() % 100 / 100.0f;
        }
    }
}
