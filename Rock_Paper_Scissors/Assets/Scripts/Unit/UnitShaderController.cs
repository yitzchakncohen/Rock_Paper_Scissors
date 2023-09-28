using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RockPaperScissors.Units
{
    public class UnitShaderController : MonoBehaviour {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private float levelUpAnimationTime = 0.5f;
        
        public void SetupSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public void SetOutlineOn()
        {
            spriteRenderer.material.SetInt("_OutlineOn", 1);
        }

        public void SetOutlineOff()
        {
            spriteRenderer.material.SetInt("_OutlineOn", 0);
        }

        public IEnumerator AnimateLevelUp()
        {   
            float timer = levelUpAnimationTime;
            float animationValue = 0;

            while(timer > 0)
            {
                timer -= Time.deltaTime;
                animationValue = 1 - (timer/levelUpAnimationTime);
                spriteRenderer.material.SetFloat("_LevelUpValue", animationValue);

                yield return null;
            }

            spriteRenderer.material.SetFloat("_LevelUpValue", 0);

        }
    }
}