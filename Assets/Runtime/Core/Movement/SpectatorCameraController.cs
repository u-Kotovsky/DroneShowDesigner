using UnityEngine;

namespace Runtime.Core.Movement
{
    public class SpectatorCameraController : MonoBehaviour
    {
        private float movementSpeed = 1f;
        private float movementSpeedBoostMultiplier = 8f;

        private bool isMovementEnabled = true;
        private Vector2 mouseLook = new(0f, 0f);
        private Vector3 movement = new(0f, 0f, 0f);
        
        private bool isSprinting;

        private bool isRightMouseDown;
        private bool IsRightMouseDown
        {
            get => isRightMouseDown;
            set
            {
                if (isRightMouseDown != value) Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                isRightMouseDown = value;
            }
        }

        public void EnableMovement()
        {
            isMovementEnabled = true;
        }

        public void DisableMovement()
        {
            isMovementEnabled = false;
        }

        private void Update()
        {
            if (!isMovementEnabled) return;

            IsRightMouseDown = Input.GetKey(KeyCode.Mouse1);
            
            mouseLook.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Up/Down"), Input.GetAxis("Vertical"));
            
            isSprinting = Input.GetKey(KeyCode.LeftShift);

            if (!IsRightMouseDown) return;
            
            transform.Translate(movement * (movementSpeed * (isSprinting ? movementSpeedBoostMultiplier : 1)), Space.Self);
            
            var rotation = transform.rotation;
            var horizontal = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
            var vertical = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            
            transform.rotation = horizontal * rotation * vertical;
        }
    }
}
