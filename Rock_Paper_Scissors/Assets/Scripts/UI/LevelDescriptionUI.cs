using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RockPaperScissors;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;

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
    private GameplayManager gameplayManager;

    private void Start() 
    {
        gameplayManager = FindObjectOfType<GameplayManager>();
    }

    private void OnEnable() 
    {
        LevelData levelData = gameplayManager.GetLevelData(gameplayManager.GameMode, gameplayManager.Level);
        Setup(levelData, gameplayManager.Level);
    }

    private void Setup(LevelData levelData, int level)
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

    private void AnimateSetup(LevelData levelData, int level)
    {
        float animationTime = 0.5f;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(animationTime);
        sequence.AppendCallback(() => {
            levelText.GetComponent<LetterAnimation>().Play(LEVEL + level, animationTime);
            AudioManager.Instance.PlayWritingSound();
        });
        sequence.AppendInterval(animationTime);
        sequence.AppendCallback(() => {
            scoreRequirementOneStar.GetComponent<LetterAnimation>().Play(NO_REQUIREMENT, animationTime);
            AudioManager.Instance.PlayWritingSound();
        });
        sequence.AppendInterval(animationTime);
        sequence.AppendCallback(() => {
            scoreRequirementTwoStar.GetComponent<LetterAnimation>().Play(SCORE_REQUIREMENT_1 + levelData.twoStarRequirement + SCORE_REQUIREMENT_2, animationTime);
            AudioManager.Instance.PlayWritingSound();
        });
        sequence.AppendInterval(animationTime);
        sequence.AppendCallback(() => {
            scoreRequirementThreeStar.GetComponent<LetterAnimation>().Play(SCORE_REQUIREMENT_1 + levelData.threeStarRequirement + SCORE_REQUIREMENT_2, animationTime);
            AudioManager.Instance.PlayWritingSound();
        });
        sequence.PlayForward();
    }
}
