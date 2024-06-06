using DG.Tweening;
using UnityEngine;

namespace RockPaperScissors.UI.Components
{
    public class ModalWindow : MonoBehaviour
    {
        private float openModalAnimationTime = 0.3f;
        private float closeModalAnimationTime = 0.3f;
        private RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Open()
        {
            AudioManager.Instance.PlayMenuNavigationSound();
            gameObject.SetActive(true);
            transform.localPosition = new Vector2(0, -Screen.height);
            Sequence openModalSequence = DOTween.Sequence();
            openModalSequence.Append(rectTransform.DOAnchorPos(Vector2.zero, openModalAnimationTime).SetEase(Ease.InOutQuint)).SetUpdate(true);
            openModalSequence.PlayForward();
        }

        public void Close()
        {
            AudioManager.Instance.PlayMenuNavigationSound();
            Sequence openModalSequence = DOTween.Sequence();
            openModalSequence.Append(rectTransform.DOAnchorPos(new Vector2(0, -Screen.height), closeModalAnimationTime).SetEase(Ease.InOutQuint)).SetUpdate(true);
            openModalSequence.AppendCallback(() => {gameObject.SetActive(false);}).SetUpdate(true);
            openModalSequence.PlayForward();
        }
    }
}
