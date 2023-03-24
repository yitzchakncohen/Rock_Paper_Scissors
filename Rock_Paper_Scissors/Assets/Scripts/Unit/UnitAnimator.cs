using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private SpriteResolver spriteResolver;
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

    public void MoveLeft()
    {
        spriteResolver.SetCategoryAndLabel("Left", "Level 1");
        // animator.SetTrigger("Left");
    }

    public void MoveRight()
    {
        spriteResolver.SetCategoryAndLabel("Right", "Level 1");
        // animator.SetTrigger("Right");
    }

    public void MoveUpLeft()
    {
        spriteResolver.SetCategoryAndLabel("UpLeft", "Level 1");
        // animator.SetTrigger("Up_Left");
    }

    public void MoveUpRight()
    {
        spriteResolver.SetCategoryAndLabel("UpRight", "Level 1");
        // animator.SetTrigger("Up_Right");
    }

    public void MoveDownLeft()
    {
        spriteResolver.SetCategoryAndLabel("DownLeft", "Level 1");
        // animator.SetTrigger("Down_Left");
    }

    public void MoveDownRight()
    {
        spriteResolver.SetCategoryAndLabel("DownRight", "Level 1");
        // animator.SetTrigger("Down_Right");
    }

    public void ToggleMoveAnimation(bool isMoving)
    {
        animator.SetBool("Move", isMoving);
    }
}
