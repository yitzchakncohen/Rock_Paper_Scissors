using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private UnitProgression unitProgression;
    [SerializeField] private SpriteResolver spriteResolver;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject spawnFXPrefab;
    private SpriteLibrary spriteLibrary;
    private Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
    }

    private void Start() 
    {
        unitProgression.OnLevelUp += UnitProgression_OnLevelUp;
    }

    private void OnDestroy() 
    {
        unitProgression.OnLevelUp -= UnitProgression_OnLevelUp;
    }

    public void SetSpriteLibraryAsset(SpriteLibraryAsset spriteLibraryAsset)
    {
        spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
    }

    public void MoveLeft()
    {
        spriteResolver.SetCategoryAndLabel("Left", GetLevel());
        // animator.SetTrigger("Left");
    }

    public void MoveRight()
    {
        spriteResolver.SetCategoryAndLabel("Right", GetLevel());
        // animator.SetTrigger("Right");
    }

    public void MoveUpLeft()
    {
        spriteResolver.SetCategoryAndLabel("UpLeft", GetLevel());
        // animator.SetTrigger("Up_Left");
    }

    public void MoveUpRight()
    {
        spriteResolver.SetCategoryAndLabel("UpRight", GetLevel());
        // animator.SetTrigger("Up_Right");
    }

    public void MoveDownLeft()
    {
        spriteResolver.SetCategoryAndLabel("DownLeft", GetLevel());
        // animator.SetTrigger("Down_Left");
    }

    public void MoveDownRight()
    {
        spriteResolver.SetCategoryAndLabel("DownRight", GetLevel());
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
        GameObject spawnFX = Instantiate(spawnFXPrefab, transform.position, Quaternion.identity);
        float timer = 0;
        while(timer < animationTime)
        {
            timer += Time.deltaTime;
            spriteRenderer.material.SetFloat("_DissolveValue", timer/animationTime);
            yield return new WaitForEndOfFrame();
        }
        Destroy(spawnFX);
    }

    private string GetLevel()
    {
        return $"Level {unitProgression.GetLevel()}";
    }

    private void UnitProgression_OnLevelUp()
    {
        AnimateLevelUp();
    }

    private void AnimateLevelUp()
    {
        MoveLeft();
    }
}
