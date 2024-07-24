using System;
using System.Globalization;
using System.Text.RegularExpressions;
using RockPaperScissors.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class SelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] collapsableObjects;
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private Button collapseButton;
        [SerializeField] private GameObject openImage;
        [SerializeField] private GameObject collapseImage;


        [Header("Stats")]
        [SerializeField] private GameObject background;
        [SerializeField] private TextMeshProUGUI unit;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI attack;
        [SerializeField] private TextMeshProUGUI range;
        [SerializeField] private TextMeshProUGUI defense;
        [SerializeField] private TextMeshProUGUI movement;
        [SerializeField] private TextMeshProUGUI xp;
        [SerializeField] private RectTransform preferredWidth;
        private Unit selectedUnit = null;
        private bool show = false;
        private static TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;

        private void Start() 
        {
            ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
            collapseButton.onClick.AddListener(ToggleDetails);
            background.SetActive(false);
            ShowHideDetails(show);
        }

        private void OnDestroy() 
        {
            ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
        }

        private void ToggleDetails()
        {
            show = !show;
            ShowHideDetails(show);
        }

        private void ShowHideDetails(bool show)
        {
            foreach (GameObject item in collapsableObjects)
            {
                item.SetActive(show);
            }
            if (show)
            {
                collapseImage.SetActive(true);
                openImage.SetActive(false);
                UpdateGrid();
            }
            else
            {
                collapseImage.SetActive(false);
                openImage.SetActive(true);
            }
        }

        private void UpdateGrid()
        {
            float width = LayoutUtility.GetPreferredWidth(preferredWidth);
            grid.cellSize = new Vector2(width / 2 - grid.spacing.x, grid.cellSize.y);
        }

        private void ActionHandler_OnUnitSelected(object sender, Unit unit)
        {
            if(unit != null)
            {
                background.SetActive(true);
                UpdateUnitStats(unit);
                selectedUnit = unit;
                selectedUnit.UnitProgression.OnLevelUp += selectedUnit_OnLevelUp;
                selectedUnit.UnitProgression.OnGainXP += selectedUnit_OnGainXP;
            }
            else
            {
                background.SetActive(false);
                if(selectedUnit != null)
                {
                    selectedUnit.UnitProgression.OnLevelUp -= selectedUnit_OnLevelUp;
                    selectedUnit.UnitProgression.OnGainXP -= selectedUnit_OnGainXP;
                    selectedUnit = null;
                }
            }
        }

        private void UpdateUnitStats(Unit unit)
        {
            if(unit == null)
            {
                background.SetActive(false);
                return;
            }
            this.unit.text = SplitCamelCase(unit.Class.ToString());
            level.text = $"Level: {unit.GetLevel()}";
            health.text = $"<sprite=0> {unit.Health}/{unit.GetMaximumHealth()}";
            attack.text = $"<sprite=2> {unit.AttackDamage}";
            range.text = $"<sprite=3> {unit.AttackRange}";
            defense.text = $"<sprite=1> {unit.Defense}";
            movement.text = $"<sprite=4> {unit.MoveDistance}";
            if(unit.XPToLevelUp < 0)
            {
                xp.text = "XP -";
            }
            else
            {
                xp.text = $"XP {unit.UnitProgression.XP}/{unit.XPToLevelUp}";
            }
            UpdateGrid();
        }

        private void selectedUnit_OnLevelUp()
        {
            UpdateUnitStats(selectedUnit);
        }

        private void selectedUnit_OnGainXP()
        {
            UpdateUnitStats(selectedUnit);
        }

        private string SplitCamelCase(string str)
        {
            string newString = Regex.Replace(str, @"\p{Lu}", m => " " + m.Value.ToLower());     
            newString = textInfo.ToTitleCase(newString);
            return newString.Trim();
        }
    }
}
