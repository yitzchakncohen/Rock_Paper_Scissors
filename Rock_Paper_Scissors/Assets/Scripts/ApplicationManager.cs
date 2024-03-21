using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RockPaperScissors
{
    /// <summary>
    /// Class <c>ApplicationManager</c> is a persistent class that manages the high level state of the application. 
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        public static ApplicationManager Instance;
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void StartNewGame()
        {
            SceneManager.LoadScene(1);
        }

        public void ContinueGame()
        {

        }
    }

}
