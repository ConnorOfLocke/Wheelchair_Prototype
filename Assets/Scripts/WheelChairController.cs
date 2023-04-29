using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using UnityEngine.InputSystem;
using Utils;

namespace Wheelchair
{
    public class WheelChairController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float speed = 5.0f;
        [SerializeField]
        private float friction = 20.0f;
        [SerializeField]
        private float maxVelocity = 20.0f;

        [SerializeField]
        private float rotationSpeed = 10.0f;
        [SerializeField]
        private float rotationalFriction = 5.0f;
        [SerializeField]
        private float maxRotationVelocity = 2.5f;

        [SerializeField, ManagedInputAction]
        private string moveActionID = "";

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private bool moveInputDown = false;
        [SerializeField, ReadOnly]
        private float velocity = 0.0f;
        [SerializeField, ReadOnly]
        private float angularVelocity = 0.0f;

        private InputAction moveInputAction = null;

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
                ApplyInput(inputVector);
            }

            UpdateMovement();
        }

        private void HandleInput(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action.id.ToString() == moveActionID)
            {
                moveInputDown = callbackContext.phase == InputActionPhase.Performed;
            }
        }

        private void ApplyInput(Vector3 inputVector)
        {
            float addedSpeed = inputVector.y * speed * Time.deltaTime;
            float addedRotation = inputVector.x * rotationSpeed * Time.deltaTime;

            velocity = Mathf.Clamp(velocity + addedSpeed, -maxVelocity, maxVelocity);
            angularVelocity = Mathf.Clamp(angularVelocity + addedRotation, -maxRotationVelocity, maxRotationVelocity);
        }


        private void UpdateMovement()
        {
            transform.rotation = transform.rotation * Quaternion.AngleAxis(angularVelocity, Vector3.up);
            transform.position += transform.forward * velocity;

            //slow down speed
            velocity = ApplyFriction(velocity, friction);
            angularVelocity = ApplyFriction(angularVelocity, rotationalFriction);
        }

        private float ApplyFriction(float velocity, float friction)
        {
            float newVelocity = velocity;
            if (newVelocity != 0)
            {
                float addedFriction = ((newVelocity > 0) ? -friction : friction) * Time.deltaTime;

                //if the friction is more than the velocity, set it to 0
                if ((newVelocity > 0 && newVelocity + addedFriction < 0) ||
                    newVelocity < 0 && newVelocity + addedFriction > 0)
                {
                    newVelocity = 0;
                }
                else
                {
                    newVelocity += addedFriction;
                }
            }

            return newVelocity;
        }
    }
}
