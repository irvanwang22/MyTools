using UnityEngine;

namespace MyTools
{
    /// <summary>
    /// A component that makes a GameObject always face the camera, creating a billboard effect.
    /// </summary>
    public class SpriteBillboard : MonoBehaviour
    {
        [Header("Rotation Axis Settings")]
        [Tooltip("Allow rotation on X axis")]
        public bool rotateOnX = true;
        
        [Tooltip("Allow rotation on Y axis")]
        public bool rotateOnY = true;
        
        [Tooltip("Allow rotation on Z axis")]
        public bool rotateOnZ = true;

        private Camera cameraRef;

        private void Awake()
        {
            cameraRef = Camera.main;
        }

        private void LateUpdate()
        {
            if (cameraRef == null)
                return;

            // Calculate direction towards camera
            Vector3 directionToCamera = cameraRef.transform.position - transform.position;
            
            // Calculate rotation based on direction
            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
            
            // Get current and target euler angles
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;
            
            // Selectively apply rotation based on settings
            Vector3 finalEuler = new Vector3(
                rotateOnX ? targetEuler.x : currentEuler.x,
                rotateOnY ? targetEuler.y : currentEuler.y,
                rotateOnZ ? targetEuler.z : currentEuler.z
            );
            
            // Apply final rotation
            transform.rotation = Quaternion.Euler(finalEuler);
        }
    }
}