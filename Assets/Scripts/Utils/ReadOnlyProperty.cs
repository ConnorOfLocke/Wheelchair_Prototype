using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    { }
}
