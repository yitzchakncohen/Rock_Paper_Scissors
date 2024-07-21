using System.Collections;
using System.Collections.Generic;
using RockPaperScissors;
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
    GameplayManager gameplayManager;

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
        levelText.text = LEVEL + level;
        scoreRequirementOneStar.text = NO_REQUIREMENT;
        scoreRequirementTwoStar.text = SCORE_REQUIREMENT_1 + levelData.twoStarRequirement + SCORE_REQUIREMENT_2;
        scoreRequirementThreeStar.text = SCORE_REQUIREMENT_1 + levelData.threeStarRequirement + SCORE_REQUIREMENT_2;
    }
}
