using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public class InputManager : Singleton<InputManager>
    {
        #region Fields

        [Header("References")]
        [SerializeField]
        private InputActionAsset playerInputActions = null;

        #endregion

        public InputActionAsset PlayerInputActions => playerInputActions;

        protected override void Initialise()
        { }

        public InputAction GetInputActionFromID(string inputID)
        {
            Guid id = new Guid(inputID);

            if (playerInputActions != null)
            {
                foreach (InputActionMap actionMap in playerInputActions.actionMaps)
                {
                    foreach (InputAction action in actionMap)
                    {
                        if (action.id == id)
                            return action;
                    }
                }
            }

            return null;
        }
    }
}
