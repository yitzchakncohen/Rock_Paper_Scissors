using RockPaperScissors.Units;
using TMPro;
using UnityEngine;

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
        
        private void Start() 
        {
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            background.SetActive(false);
        }

        private void ActionHandler_OnUnitSelected(object sender, Unit unit)
        {
            if(unit != null)
            {
                background.SetActive(true);
                this.unit.text = unit.GetUnitClass().ToString();
                level.text = $"Level: {unit.GetLevel().ToString()}";
                health.text = $" <sprite=0> {unit.GetHealth().ToString()}/{unit.GetMaximumHealth().ToString()}";
                attack.text = $" <sprite=2> {unit.GetBaseAttack()}";
                range.text = $" <sprite=3> {unit.GetAttackRange()}";
                defense.text = $" <sprite=1> {unit.GetBaseDefense()}";
            }
            else
            {
                background.SetActive(false);
            }
        }
    }
}
