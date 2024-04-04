using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class RewardBonusUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardValueText;

        private void Awake() 
        {
            gameObject.SetActive(false);
        }

        public void SetRewardAmount(int rewardAmount)
        {
            rewardValueText.text = "+" + rewardAmount.ToString();
        }
    }    
}
