using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
    }

    public void MoveLeft()
    {
        animator.SetTrigger("Left");
    }

    public void MoveRight()
    {
        animator.SetTrigger("Right");
    }

    public void MoveUpLeft()
    {
        animator.SetTrigger("Up_Left");
    }

    public void MoveUpRight()
    {
        animator.SetTrigger("Up_Right");
    }

    public void MoveDownLeft()
    {
        animator.SetTrigger("Down_Left");
    }

    public void MoveDownRight()
    {
        animator.SetTrigger("Down_Right");
    }
}
