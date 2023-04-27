using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelchairAnim
{
    public class WheelChairAnim : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private List<Wheel> wheels = null;

        [Header("Settings")]
        [SerializeField]
        private float angleSpeed = 10.0f;

        private void Awake()
        {
            if (wheels != null)
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].Init();
                }
            }
        }

        void Update()
        {
            UpdateWheels();
        }

        private void UpdateWheels()
        {
            if (wheels != null)
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].UpdateWheel(transform, angleSpeed);
                    wheels[i].UpdateArmPosition();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right);

            if (wheels != null)
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].DrawGizmos();
                }
            }
        }
    }
}
