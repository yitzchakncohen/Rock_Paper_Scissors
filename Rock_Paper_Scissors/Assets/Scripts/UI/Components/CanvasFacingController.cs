using UnityEngine;

namespace RockPaperScissors.UI.Components
{
    public class CanvasFacingController : MonoBehaviour
    {
        private Transform mainCameraTransform;

        private void Start() 
        {
            mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate() 
        {
            transform.LookAt(mainCameraTransform);
            transform.forward = -transform.forward;
        }
    }
}
