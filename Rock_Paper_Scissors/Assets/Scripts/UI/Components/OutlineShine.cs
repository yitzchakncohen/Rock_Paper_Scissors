using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineShine : MonoBehaviour
    {
        [SerializeField] private float shineAnimationTime = 0.3f;
        [SerializeField] private bool randomShineEnabled = true;
        private bool shining = false;
        private SpriteRenderer spriteRenderer;
        private float shineValue = 0f;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material.SetFloat("_x1", shineValue);
            if(randomShineEnabled)
            {
                shining = true;
                StartShine(5.0f, 15.0f);
            }
        }

        public void SetupSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public void StartShine(float minRandomTime, float maxRandomTime)
        {
            shining = true;
            StartCoroutine(Shine(minRandomTime, maxRandomTime));
        }

        public void StopShine()
        {
            StopAllCoroutines();
            spriteRenderer.material.SetFloat("_x1", 0f);
            shining = false;
        }

        private IEnumerator Shine(float minRandomTime, float maxRandomTime)
        {
            yield return new WaitForSeconds(Random.Range(0f, maxRandomTime));
            while(shining)
            {
                spriteRenderer.material.SetFloat("_x1", shineValue);
                shineValue += Time.deltaTime * (1f/shineAnimationTime);
                if(shineValue > 2.0f)
                {
                    shineValue = 0f;
                    spriteRenderer.material.SetFloat("_x1", shineValue);
                    yield return new WaitForSeconds(Random.Range(minRandomTime, maxRandomTime));
                }
                else
                {
                    yield return null;
                }
            }
        }

        public IEnumerator StartShine(float shineAnimationTime)
        {
            shineValue = 0f;
            spriteRenderer.material.SetFloat("_x1", shineValue);
            while(shineValue < 2.0f)
            {
                shineValue += Time.deltaTime * (1f/shineAnimationTime);
                spriteRenderer.material.SetFloat("_x1", shineValue);
                yield return null;
            }
        }
    }
}
