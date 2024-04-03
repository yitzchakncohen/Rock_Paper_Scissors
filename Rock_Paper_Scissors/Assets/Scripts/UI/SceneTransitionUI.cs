using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionUI : MonoBehaviour
{
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject loadingPanel;

    public void TransitionOut()
    {
        transitionPanel.SetActive(true);
    }

    public void TransitionIn()
    {
        transitionPanel.SetActive(false);
    }

    public void StartLoading()
    {
        loadingPanel.SetActive(true);
    }

    public void LoadingCompleted()
    {
        loadingPanel.SetActive(false);
    }
}
