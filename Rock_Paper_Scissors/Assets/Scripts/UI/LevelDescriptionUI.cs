using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RockPaperScissors;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class LevelDescriptionUI : MonoBehaviour
    {
        const string SCORE_REQUIREMENT_1 = "In ";
        const string SCORE_REQUIREMENT_2 = " Turns";
        const string NO_REQUIREMENT = "Victory";
        const string LEVEL = "Level ";

        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreRequirementOneStar;
        [SerializeField] private TextMeshProUGUI scoreRequirementTwoStar;
        [SerializeField] private TextMeshProUGUI scoreRequirementThreeStar;
        [SerializeField] private bool animateOnOpen = false;
        private Sequence animationSequence;

        public void Setup(LevelData levelData, int level)
        {
            if(animateOnOpen)
            {
                AnimateSetup(levelData, level);
            }
            else
            {
                levelText.text = LEVEL + level;
                scoreRequirementOneStar.text = NO_REQUIREMENT;
                scoreRequirementTwoStar.text = SCORE_REQUIREMENT_1 + levelData.twoStarRequirement + SCORE_REQUIREMENT_2;
                scoreRequirementThreeStar.text = SCORE_REQUIREMENT_1 + levelData.threeStarRequirement + SCORE_REQUIREMENT_2;
            }
        }

        private void OnDisable() 
        {
            animationSequence.Kill();
        }

        private void AnimateSetup(LevelData levelData, int level)
        {
            float animationTime = 0.5f;
            animationSequence = DOTween.Sequence();
            animationSequence.AppendInterval(animationTime);
            animationSequence.AppendCallback(() => {
                levelText.GetComponent<LetterAnimation>().Play(LEVEL + level, animationTime);
                AudioManager.Instance.PlayWritingSound();
            });
            animationSequence.AppendInterval(animationTime);
            animationSequence.AppendCallback(() => {
                scoreRequirementOneStar.GetComponent<LetterAnimation>().Play(NO_REQUIREMENT, animationTime);
                AudioManager.Instance.PlayWritingSound();
            });
            animationSequence.AppendInterval(animationTime);
            animationSequence.AppendCallback(() => {
                scoreRequirementTwoStar.GetComponent<LetterAnimation>().Play(SCORE_REQUIREMENT_1 + levelData.twoStarRequirement + SCORE_REQUIREMENT_2, animationTime);
                AudioManager.Instance.PlayWritingSound();
            });
            animationSequence.AppendInterval(animationTime);
            animationSequence.AppendCallback(() => {
                scoreRequirementThreeStar.GetComponent<LetterAnimation>().Play(SCORE_REQUIREMENT_1 + levelData.threeStarRequirement + SCORE_REQUIREMENT_2, animationTime);
                AudioManager.Instance.PlayWritingSound();
            });
            animationSequence.PlayForward();
        }
    }
}
