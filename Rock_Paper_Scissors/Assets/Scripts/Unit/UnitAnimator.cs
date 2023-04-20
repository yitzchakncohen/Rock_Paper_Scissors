using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private SpriteResolver spriteResolver;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject spawnFXPrefab;
    [SerializeField] private GameObject healthBar;
    private SpriteLibrary spriteLibrary;
    private Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
    }

    public void SetSpriteLibraryAsset(SpriteLibraryAsset spriteLibraryAsset)
    {
        spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
    }

    public void MoveLeft(int level)
    {
        spriteResolver.SetCategoryAndLabel("Left", GetLevel(level));
        // animator.SetTrigger("Left");
    }

    public void MoveRight(int level)
    {
        spriteResolver.SetCategoryAndLabel("Right", GetLevel(level));
        // animator.SetTrigger("Right");
    }

    public void MoveUpLeft(int level)
    {
        spriteResolver.SetCategoryAndLabel("UpLeft", GetLevel(level));
        // animator.SetTrigger("Up_Left");
    }

    public void MoveUpRight(int level)
    {
        spriteResolver.SetCategoryAndLabel("UpRight", GetLevel(level));
        // animator.SetTrigger("Up_Right");
    }

    public void MoveDownLeft(int level)
    {
        spriteResolver.SetCategoryAndLabel("DownLeft", GetLevel(level));
        // animator.SetTrigger("Down_Left");
    }

    public void MoveDownRight(int level)
    {
        spriteResolver.SetCategoryAndLabel("DownRight", GetLevel(level));
        // animator.SetTrigger("Down_Right");
    }

    public void ToggleMoveAnimation(bool isMoving)
    {
        animator.SetBool("Move", isMoving);
    }

    public IEnumerator DeathAnimationRoutine(float animationTime)
    {
        float timer = animationTime;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            spriteRenderer.material.SetFloat("_DissolveValue", timer/animationTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator SpawnAnimationRoutine(float animationTime)
    {
        HideUnit();
        GameObject spawnFX = Instantiate(spawnFXPrefab, transform.position, Quaternion.identity);
        float timer = 0;
        while(timer < animationTime)
        {
            timer += Time.deltaTime;
            spriteRenderer.material.SetFloat("_DissolveValue", timer/animationTime);
            yield return new WaitForEndOfFrame();
        }
        Destroy(spawnFX);
        healthBar.gameObject.SetActive(true);
    }

    public void HideUnit()
    {
        spriteRenderer.material.SetFloat("_DissolveValue", 0f);
        healthBar.gameObject.SetActive(false);
    }

    private string GetLevel(int level)
    {
        return $"Level {level}";
    }

    public void AnimateLevelUp(int level)
    {
        MoveLeft(level);
    }
}
