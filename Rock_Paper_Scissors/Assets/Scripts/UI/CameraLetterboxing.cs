using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI
{
    [RequireComponent(typeof(Camera))]
    public class CameraLetterboxing : MonoBehaviour
    {
        private Camera cameraComponent;
        private float aspectRatio;

        void Awake()
        {
            cameraComponent = GetComponent<Camera>();
            SetupCamera();
            UpdateAspectRatio();
        }

        private void FixedUpdate() 
        {
            if(aspectRatio != (float)Screen.width / (float)Screen.height)
            {
                SetupCamera();
                UpdateAspectRatio();
            }
        }

        
        private void UpdateAspectRatio()
        {
            aspectRatio = (float)Screen.width / (float)Screen.height;
        }

        private void SetupCamera()
        {
            if(aspectRatio > 2f)
            {
                float cameraWidth = Screen.height * 2f / Screen.width;
                float cameraX = (Screen.width - Screen.width * cameraWidth) / (Screen.width * 2f);
                cameraComponent.rect = new Rect
                {
                    x = 0,
                    y = 0,
                    width = 1f,
                    height = 1f
                };
            }
            else
            {
                float cameraHeight = Screen.width / (Screen.height * 2f);
                float cameraY = (Screen.height - Screen.height * cameraHeight) / (Screen.height * 2f);
                cameraComponent.rect = new Rect
                {
                    x = 0,
                    y = cameraY,
                    width = 1f,
                    height = cameraHeight
                };
            }
        }
    }    
}
