using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace RockPaperScissors
{
    /// <summary>
    /// <c>GameplayManager</c> manages the game flow, including identifying end game states. 
    /// </summary>
    public class GameplayManager : MonoBehaviour, ISaveInterface<SaveGameplayManagerData>
    {
        public class OnGameOverEventArgs : EventArgs
        {
            public int Score;
            public int Highscore;
            public LevelData LevelData;
            public GameMode GameMode;
            public bool WinCondition;
        }
        public static event EventHandler<OnGameOverEventArgs> OnGameOver;
        public static event EventHandler<OnGameOverEventArgs> OnLevelCompleted;
        public static event Action<int> OnScoreChange;
        public static event Action<int> OnNewHighscore;
        [SerializeField] private GameObject scoreFXPrefab;
        private int score = 0;
        private UnitManager unitManager;
        private TurnManager turnManager;
        public GameMode GameMode { get; private set;}
        public int Level => level;
        private int level = -1;
        [SerializeField] private LevelData endlessModeLevelData;
        [SerializeField] private List<LevelData> levelDataList;
        
        private void Awake() 
        {
            UnitHealth.OnDeath += UnitHealth_OnDeath;
            SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        }

        private void SceneManager_SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            unitManager = FindObjectOfType<UnitManager>();
            turnManager = FindObjectOfType<TurnManager>();
        }

        private void OnDestroy() 
        {
            UnitHealth.OnDeath -= UnitHealth_OnDeath;
        }

        private void UnitHealth_OnDeath(object sender, Unit attacker)
        {
            if(attacker == null)
            {
                // Self Destruct
                return;
            }

            UnitHealth unitHealth = sender as UnitHealth;
            // Game ends when the pillow fort is destroyed.
            if(unitHealth != null && unitHealth.Unit.Class == UnitClass.PillowFort)
            {
                GameOver();
            }

            // Score points for defeating enemies.
            if (attacker.IsFriendly)
            {
                score += 10;
                OnScoreChange?.Invoke(score);
                Instantiate(scoreFXPrefab, attacker.transform.position, Quaternion.identity);
            }

            if(GameMode == GameMode.Level)
            {
                if(unitManager.GetEnemyUnitsList().Count == 0)
                {
                    LevelCompleted();
                }
            }
        }


        [ContextMenu("Game Over")]
        public void GameOver()
        {
            int highscore = -1;
            if(GameMode == GameMode.Level)
            {
                score = 0;
                highscore = PlayerPrefs.GetInt(ApplicationManager.BEST_SCORE_STRING + level.ToString(), -1);
                if(highscore < score)
                {
                    PlayerPrefs.SetInt(ApplicationManager.BEST_SCORE_STRING + level.ToString(), score);
                    highscore = score;
                    OnNewHighscore?.Invoke(score);
                }
            }
            else
            {
                highscore = PlayerPrefs.GetInt(ApplicationManager.HIGH_SCORE_STRING, -1);
                if(highscore < score)
                {
                    PlayerPrefs.SetInt(ApplicationManager.HIGH_SCORE_STRING, score);
                    highscore = score;
                    OnNewHighscore?.Invoke(score);
                }
            }

            LevelData levelData = GameMode == GameMode.Level ? levelDataList[level -1] : endlessModeLevelData;
            OnGameOverEventArgs onGameOverEventArgs = new OnGameOverEventArgs
            {
                Score = score, 
                Highscore = highscore,
                GameMode = this.GameMode,
                WinCondition = false,
                LevelData = levelData
            };
            AudioManager.Instance.PlayGameOverSound();
            OnGameOver?.Invoke(this, onGameOverEventArgs);
        }

        [ContextMenu("Complete Level")]
        private void LevelCompleted()
        {
            int numberOfStars = 1;
            if(turnManager.Turn <= levelDataList[level-1].twoStarRequirement)
            {
                numberOfStars = 2;
            }
            if(turnManager.Turn <= levelDataList[level-1].threeStarRequirement)
            {
                numberOfStars = 3;
            }
            int highscore = PlayerPrefs.GetInt(ApplicationManager.BEST_SCORE_STRING + level.ToString(), -1);
            if(highscore < numberOfStars)
            {
                PlayerPrefs.SetInt(ApplicationManager.BEST_SCORE_STRING + level.ToString(), numberOfStars);
                highscore = numberOfStars;
                OnNewHighscore?.Invoke(numberOfStars);
            }

            OnGameOverEventArgs onGameOverEventArgs = new OnGameOverEventArgs
            {
                Score = numberOfStars, 
                Highscore = highscore,
                GameMode = this.GameMode,
                WinCondition = true,
                LevelData = levelDataList[level -1]
            };
            AudioManager.Instance.PlayLevelCompleteSound();
            OnLevelCompleted?.Invoke(this, onGameOverEventArgs);
        }

        public SaveGameplayManagerData Save()
        {
            return new SaveGameplayManagerData
            {
                Score = score
            };
        }

        public void NextLevel()
        {
            level++;
        }

        public void StartNewGame(GameMode gameMode)
        {
            this.GameMode = gameMode;
            if(gameMode == GameMode.Endless)
            {
                level = -1;
            }
            else
            {
                level = 1;
            }
            score = 0;
        }

        public LevelData GetLevelData(GameMode gameMode, int level)
        {
            if(gameMode == GameMode.Endless)
            {
                return endlessModeLevelData;
            }
            else
            {
                return levelDataList[level - 1];
            }
        }

        public void Load(SaveGameplayManagerData loadData)
        {
            score = loadData.Score;
            level = loadData.Level;
            GameMode = loadData.GameMode;
            OnScoreChange?.Invoke(score);
        }
    }
}
