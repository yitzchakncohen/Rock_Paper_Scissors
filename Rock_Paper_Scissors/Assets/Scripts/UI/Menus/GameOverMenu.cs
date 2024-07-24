using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    public class GameOverMenu : MonoBehaviour
    {
        public static event Action OnStartEndlessGameButtonPress;
        public static event Action OnRestartLevelButtonPress;
        public static event Action OnNextLevelButtonPress;
        private const string GAME_OVER_STRING = "Game Over";
        private const string LEVEL_COMPLETE_STRING_1 = "Level ";
        private const string LEVEL_COMPLETE_STRING_2 = " Completed!";
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TextMeshProUGUI scoreValueText;
        [SerializeField] private TextMeshProUGUI highScoreValueText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI previousBestText;
        [SerializeField] private LetterAnimation textAnimation;
        [SerializeField] private float gameOverAnimationTime = 0.5f;
        [SerializeField] private float titleAnimationTime = 0.3f;
        [SerializeField] private GameObject endlessModeScore;
        [SerializeField] private GameObject endlessModeHighScore;
        [SerializeField] private GameObject levelModeScore;
        [SerializeField] private GameObject levelModeBestScore;
        [SerializeField] private LevelDescriptionUI levelDescriptionUI;
        [SerializeField] private LevelScoreIndicator[] scoreIndicators = new LevelScoreIndicator[3];
        [SerializeField] private LevelScoreIndicator[] bestScoreIndicators = new LevelScoreIndicator[3];
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start() 
        {
            newGameButton.onClick.AddListener(StartGame);
            restartLevelButton.onClick.AddListener(RestartLevel);
            nextLevelButton.onClick.AddListener(NextLevel);
        }

        private void OnDestroy() 
        {
            newGameButton.onClick.RemoveAllListeners();
            restartLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
        }

        public void Open(int score, int highscore, GameMode gameMode, int level, LevelData levelData, bool winCondition = false)
        {
            gameObject.SetActive(true);
            switch (gameMode)
            {
                case GameMode.Level:
                    SetupForLevelMode(score, highscore, levelData, level);
                    break;
                case GameMode.Endless:
                default:
                    SetupForEndlessMode();
                    break;
            }
            scoreValueText.text = score.ToString();
            highScoreValueText.text = highscore.ToString();
            rectTransform.transform.localPosition = new Vector2(0, -Screen.height);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * Camera.main.aspect/2, rectTransform.sizeDelta.y);
            Sequence gameOverSequence = DOTween.Sequence();
            gameOverSequence.Append(rectTransform.DOAnchorPos(Vector2.zero, gameOverAnimationTime).SetEase(Ease.InOutQuint).SetUpdate(true));
            if(winCondition)
            {
                gameOverSequence.AppendCallback(() => {
                    textAnimation.Play(LEVEL_COMPLETE_STRING_1 + level.ToString() + LEVEL_COMPLETE_STRING_2, titleAnimationTime);
                }).SetUpdate(true);
            }
            else
            {
                gameOverSequence.AppendCallback(() => {
                    textAnimation.Play(GAME_OVER_STRING, titleAnimationTime);
                }).SetUpdate(true);
            }
            gameOverSequence.PlayForward();
        }

        private void SetupForEndlessMode()
        {
            newGameButton.gameObject.SetActive(true);
            endlessModeScore.SetActive(true);
            endlessModeHighScore.SetActive(true);
            restartLevelButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            levelModeScore.SetActive(false);
            levelModeBestScore.SetActive(false);
            highScoreText.gameObject.SetActive(true);
            previousBestText.gameObject.SetActive(false);
            levelDescriptionUI.gameObject.SetActive(false);
        }

        private void SetupForLevelMode(int score, int highscore, LevelData levelData, int level)
        {
            int stars = CalculateStars(score, levelData);
            int bestStars = CalculateStars(highscore, levelData);

            newGameButton.gameObject.SetActive(false);
            endlessModeScore.SetActive(false);
            endlessModeHighScore.SetActive(false);
            restartLevelButton.gameObject.SetActive(true);
            if (bestStars > 0)
            {
                nextLevelButton.gameObject.SetActive(true);
            }
            else
            {
                nextLevelButton.gameObject.SetActive(false);
            }
            highScoreText.gameObject.SetActive(false);
            previousBestText.gameObject.SetActive(true);
            levelModeScore.SetActive(true);
            levelModeBestScore.SetActive(true);
            levelDescriptionUI.gameObject.SetActive(true);
            levelDescriptionUI.Setup(levelData, level);
            StartCoroutine(AnimateScoreIndicators(stars, bestStars, levelData));
        }

        private static int CalculateStars(int score, LevelData levelData)
        {
            // Zero stars if you have never beaten the level.
            if(score < 0)
            {
                return 0;
            }
            int numberOfStars = 1;
            if (score <= levelData.twoStarRequirement)
            {
                numberOfStars = 2;
            }
            if (score <= levelData.threeStarRequirement)
            {
                numberOfStars = 3;
            }
            return numberOfStars;
        }

        private IEnumerator AnimateScoreIndicators(int stars, int bestStars, LevelData levelData)
        {
            float timeBetween = 0.5f;
            WaitForSeconds wait = new WaitForSeconds(timeBetween);
            yield return wait;
            yield return wait;
            scoreIndicators[0].UpdateScore(stars >= 1);
            yield return wait;
            scoreIndicators[1].UpdateScore(stars >= 2);
            yield return wait;
            scoreIndicators[2].UpdateScore(stars >= 3);
            yield return wait;
            bestScoreIndicators[0].UpdateScore(bestStars >= 1);
            yield return wait;
            bestScoreIndicators[1].UpdateScore(bestStars >= 2);
            yield return wait;
            bestScoreIndicators[2].UpdateScore(bestStars >= 3);
        }

        private void StartGame()
        {
            OnStartEndlessGameButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void RestartLevel()
        {
            OnRestartLevelButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void NextLevel()
        {
            OnNextLevelButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }
    }
}
