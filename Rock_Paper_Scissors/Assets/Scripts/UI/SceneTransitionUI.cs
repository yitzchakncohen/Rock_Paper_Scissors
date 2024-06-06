using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class SceneTransitionUI : MonoBehaviour
    {
        [SerializeField] private Image transitionPanel;
        [SerializeField] private Image loadingPanel;
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private float fadeInTime = 0.5f;

        public IEnumerator TransitionOut()
        {
            transitionPanel.gameObject.SetActive(true);
            transitionPanel.DOFade(1.0f, fadeOutTime).SetUpdate(true);
            yield return new WaitForSeconds(fadeOutTime);
        }

        public void TransitionIn()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transitionPanel.DOFade(0.0f, fadeInTime).SetUpdate(true));
            sequence.AppendCallback(()=>{
                transitionPanel.gameObject.SetActive(false);
            }).SetUpdate(true);
            sequence.PlayForward();
        }

        public void StartLoading()
        {
            loadingPanel.gameObject.SetActive(true);
            loadingPanel.DOFade(1.0f, 0.0f).SetUpdate(true);
        }

        public void LoadingCompleted()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(loadingPanel.DOFade(0.0f, fadeInTime).SetUpdate(true));
            sequence.AppendCallback(()=>{
                loadingPanel.gameObject.SetActive(false);
            }).SetUpdate(true);
            sequence.PlayForward();
        }
    }
}
