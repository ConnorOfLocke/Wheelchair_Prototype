using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    /// <summary>
    /// Property that stores the GUID's of InputActions contained in the InputManager
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ManagedInputAction : PropertyAttribute
    { }
}
