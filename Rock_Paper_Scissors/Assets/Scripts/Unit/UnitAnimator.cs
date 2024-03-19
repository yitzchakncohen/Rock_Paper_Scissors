using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace RockPaperScissors.Units
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject spawnFXPrefab;
        [SerializeField] private ParticleSystem levelUpFX;
        [SerializeField] private GameObject healthBar;
        private SpriteLibrary spriteLibrary;
        private Animator animator;
        private UnitShaderController unitShaderController;
        private Direction currentFacingDirection;

        private void Awake() 
        {
            animator = GetComponent<Animator>();
            spriteLibrary = GetComponent<SpriteLibrary>();
            unitShaderController = GetComponent<UnitShaderController>();
        }

        public void SetSpriteLibraryAsset(SpriteLibraryAsset spriteLibraryAsset)
        {
            spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
        }

        public void MoveLeft(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("Left", GetLevel(level));
            // animator.SetTrigger("Left");
            currentFacingDirection = Direction.West;
        }

        public void MoveRight(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("Right", GetLevel(level));
            // animator.SetTrigger("Right");
            currentFacingDirection = Direction.East;
        }

        public void MoveUpLeft(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("UpLeft", GetLevel(level));
            // animator.SetTrigger("Up_Left");
            currentFacingDirection = Direction.NorthWest;
        }

        public void MoveUpRight(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("UpRight", GetLevel(level));
            // animator.SetTrigger("Up_Right");
            currentFacingDirection = Direction.NorthEast;
        }

        public void MoveDownLeft(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("DownLeft", GetLevel(level));
            // animator.SetTrigger("Down_Left");
            currentFacingDirection = Direction.SouthWest;
        }

        public void MoveDownRight(int level)
        {
            if(spriteResolver == null) { return; }

            spriteResolver.SetCategoryAndLabel("DownRight", GetLevel(level));
            // animator.SetTrigger("Down_Right");
            currentFacingDirection = Direction.SouthEast;
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

        public IEnumerator AnimateLevelUp(int level)
        {
            yield return StartCoroutine(unitShaderController.AnimateLevelUp());
            SetFacingDirection(currentFacingDirection, level);
            levelUpFX.Play();
        }

        public Direction GetCurrentDirection()
        {
            return currentFacingDirection;
        }

        public void SetFacingDirection(Direction facingDirection, int level)
        {
            switch (facingDirection)
            {
                case Direction.NorthEast:
                    MoveUpRight(level);
                    break;
                case Direction.NorthWest:
                    MoveUpRight(level);
                    break;
                case Direction.SouthEast:
                    MoveDownRight(level);
                    break;
                case Direction.SouthWest:
                    MoveDownLeft(level);
                    break;
                case Direction.East:
                    MoveRight(level);
                    break;
                case Direction.West:
                default:
                    MoveLeft(level);
                    break;
            }
        }
    }
}
