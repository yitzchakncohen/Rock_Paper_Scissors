using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShine : MonoBehaviour
{
    [SerializeField] private float shineAnimationTime = 0.3f;
    [SerializeField] private bool randomShineEnabled = true;
    private SpriteRenderer spriteRenderer;
    private float shineValue = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetFloat("_x1", shineValue);
        if(randomShineEnabled)
        {
            StartCoroutine(Shine());
        }
    }

    public void SetupSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    private IEnumerator Shine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 15.0f));
        while(isActiveAndEnabled)
        {
            spriteRenderer.material.SetFloat("_x1", shineValue);
            shineValue += Time.deltaTime * (1f/shineAnimationTime);
            if(shineValue > 2.0f)
            {
                shineValue = 0f;
                spriteRenderer.material.SetFloat("_x1", shineValue);
                yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));
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
