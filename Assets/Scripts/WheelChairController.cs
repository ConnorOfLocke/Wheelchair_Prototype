using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using UnityEngine.InputSystem;

namespace Wheelchair
{
    public class WheelChairController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float speed = 10.0f;
        [SerializeField]
        private float rotationSpeed = 60.0f;

        [SerializeField, ManagedInputAction]
        private string moveActionID = "";

        private InputAction moveInputAction = null;
        private bool moveInputDown = false;

        private void Awake()
        {
            PlayerInputMediator.OnInputStarted += HandleInput;

            moveInputAction = InputManager.Instance.GetInputActionFromID(moveActionID);
        }
        private void OnDestroy()
        {
            PlayerInputMediator.OnInputStarted -= HandleInput;
        }

        private void Update()
        {
            if (moveInputDown)
            {
                Vector2 inputVector = moveInputAction.ReadValue<Vector2>();
                ApplyMovement(inputVector);
            }
        }

        private void HandleInput(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action.id.ToString() == moveActionID)
            {
                moveInputDown = callbackContext.phase == InputActionPhase.Performed;
            }
        }

        private void ApplyMovement(Vector3 inputVector)
        {
            float addedSpeed = inputVector.y * speed * Time.deltaTime;
            float addedRotation = inputVector.x * rotationSpeed * Time.deltaTime;

            transform.rotation = transform.rotation * Quaternion.AngleAxis(addedRotation, Vector3.up);
            transform.position += transform.forward * addedSpeed;
        }
    }
}
