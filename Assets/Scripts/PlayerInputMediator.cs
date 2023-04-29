using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputMediator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerInput playerInput = null;

        public delegate void OnInputAction(InputAction.CallbackContext callbackContext);
        public static event OnInputAction OnInputStarted;

        private void Awake()
        {
            playerInput.onActionTriggered += HandleActionTriggered;
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= HandleActionTriggered;
        }

        private void HandleActionTriggered(InputAction.CallbackContext callbackContext)
        {
            OnInputStarted?.Invoke(callbackContext);
        }
    }
}
