using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarImage; 
        private UnitHealth health;

        private void Awake() 
        {
            health = GetComponentInParent<UnitHealth>();
            health.OnHealthChanged += Health_OnHealthChanged;
            healthBarImage.fillAmount = 1f;
        }

        private void Health_OnHealthChanged()
        {
            healthBarImage.fillAmount = health.NormalizedHealth;
        }
    }    
}
