using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class RadialLayoutGroup : MonoBehaviour
    {
        public event Action OnCloseAnimationComplete;
        private List<Transform> children = new List<Transform>();
        private RectTransform rectTransform;
        private float animationSpeed = 0.05f;
        private float zeroPositionAngle = 90f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void GetCurrentChildren()
        {
            children.Clear();
            foreach (Transform child in transform)
            {
                if (child.parent == transform)
                {
                    children.Add(child);
                }
            }
        }

        private IEnumerator AnimateMenuOpenRoutine()
        {        
            GetCurrentChildren();

            float radius = rectTransform.sizeDelta.y / 2;
            float spacingInRadians = 360f / children.Count;

            for (int i = 0; i < children.Count; i++)
            {
                float radialPosition = spacingInRadians * i + zeroPositionAngle;
                if(i == children.Count - 1)
                {
                    yield return StartCoroutine(AnimateChild(children[i], radialPosition, zeroPositionAngle, radius));
                }
                else
                {
                    StartCoroutine(AnimateChild(children[i], radialPosition, zeroPositionAngle, radius));
                }
            }
        }

        private IEnumerator AnimateMenuClosedRoutine()
        {
            float radius = rectTransform.sizeDelta.y / 2;
            float spacingInRadians = 360f / children.Count;

            for (int i = 0; i < children.Count; i++)
            { 
                float radialPosition = spacingInRadians * i + zeroPositionAngle;
                if(i == children.Count - 1)
                {
                    yield return StartCoroutine(AnimateChild(children[i], zeroPositionAngle, radialPosition, radius));
                }
                else
                {
                    StartCoroutine(AnimateChild(children[i], zeroPositionAngle, radialPosition, radius));
                }
            }
            OnCloseAnimationComplete?.Invoke();
        }

        private IEnumerator AnimateChild(Transform child, float tragetRadialPosition, float startingRadialPosition, float radius)
        {
            if(!child.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            {
            yield break;
            }

            float currentRadialPosition = startingRadialPosition;

            do
            {
                currentRadialPosition = Mathf.Lerp(currentRadialPosition, tragetRadialPosition, animationSpeed);
                float xPosition = Mathf.Cos(currentRadialPosition * Mathf.Deg2Rad) * radius;
                float yPosition = Mathf.Sin(currentRadialPosition * Mathf.Deg2Rad) * radius;
                rectTransform.localPosition = new Vector2(xPosition, yPosition);

                if(Math.Abs(currentRadialPosition - tragetRadialPosition) < 1)
                {
                    currentRadialPosition = tragetRadialPosition;
                }
                yield return null;
            }        
            while(currentRadialPosition != tragetRadialPosition);
        }

        public void AnimateMenuOpen()
        {
            StopAllCoroutines();
            StartCoroutine(AnimateMenuOpenRoutine());
        }

        public void AnimateMenuClosed()
        {
            if(gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateMenuClosedRoutine());
            }
        }
    }    
}
