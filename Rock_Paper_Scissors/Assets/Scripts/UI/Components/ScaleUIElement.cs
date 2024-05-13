using UnityEngine;

namespace RockPaperScissors.UI.Components
{
    public class ScaleUIElement : MonoBehaviour
    {
        private CameraController cameraController;
        private float startingZoom;
        private Vector3 startingScale;

        private void Awake() 
        {
            startingScale = transform.localScale;
        }

        private void Start() 
        {
            cameraController = FindObjectOfType<CameraController>();
            startingZoom = cameraController.DefaultZoom;
        }

        private void Update() 
        {
            if(startingZoom != Camera.main.orthographicSize)
            {
                transform.localScale = startingScale * Camera.main.orthographicSize/startingZoom;
            }
        }
    }    
}
