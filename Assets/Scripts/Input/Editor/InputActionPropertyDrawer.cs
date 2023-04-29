using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Input;
using UnityEngine.InputSystem;

namespace Utils
{
    [CustomPropertyDrawer(typeof(ManagedInputAction))]
    public class InputActionPropertyDrawer : PropertyDrawer
    {
        private int selectedIndex = 0;
        private List<GUIContent> itemIds = new List<GUIContent>();
        private List<InputAction> items = new List<InputAction>();

        private static GUIStyle errorStyle = null;

        private InputManager _inputManager = null;
        private InputManager InputManager
        {
            get
            {
                if (_inputManager == null)
                {
                    _inputManager = InputManager.Instance;
                }
                return _inputManager;
            }
        }

        public InputActionPropertyDrawer()
        {
            if (errorStyle == null)
            {
                errorStyle = new GUIStyle(EditorStyles.boldLabel);
                errorStyle.normal.textColor = Color.red;
                errorStyle.wordWrap = true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //Get the PlayerInputManager Singleton           
            if (InputManager == null)
            {
                EndPropertyWithError(position, property, label, "Could not find InputManager in the scene");
                return;
            }

            if (InputManager.PlayerInputActions == null)
            {
                EndPropertyWithError(position, property, label, $"PlayerInputActions on {InputManager.gameObject.name} is null.");
                return;
            }

            //Fill the dropdown list
            foreach (InputActionMap actionMap in InputManager.PlayerInputActions.actionMaps)
            {
                foreach (InputAction action in actionMap.actions)
                {
                    itemIds.Add(new GUIContent($"{actionMap.name}/{action.name}"));
                    items.Add(action);

                    if (property.stringValue == action.id.ToString())
                    {
                        selectedIndex = itemIds.Count - 1;
                    }
                }
            }

            //Set the final value
            if (!property.serializedObject.isEditingMultipleObjects)
            {
                selectedIndex = EditorGUI.Popup(position, selectedIndex, itemIds.ToArray());
                property.stringValue = (itemIds.Count > 0) ? items[selectedIndex].id.ToString() : null;
            }
            else
            {
                EditorGUI.Popup(position, 0, new GUIContent[1] { new GUIContent("-") });
            }

            EditorGUI.EndProperty();
        }

        private InputAction GetInputAction(string inputActionString)
        {
            foreach (InputActionMap actionMap in InputManager.PlayerInputActions.actionMaps)
            {
                foreach (InputAction action in actionMap.actions)
                {
                    if (action.ToString() == inputActionString)
                    {
                        return action;
                    }
                }
            }
            return null;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private void EndPropertyWithError(Rect position, SerializedProperty property, GUIContent label, string msg)
        {
            EditorGUI.TextField(position, label, msg, errorStyle);
            EditorGUI.EndProperty();
        }
    }
}
