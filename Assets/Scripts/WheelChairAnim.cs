using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        [SerializeField]
        private bool syncArmAngles = true;
        [SerializeField, Range(0, 1)]
        private float armSyncStrength = 0.001f;

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

        private float GetWheelAverage()
        {
            return wheels.Count > 0 ? wheels.Average(wheel => wheel.ArmAngle) : 0;
        }

        private void UpdateWheels()
        {
            if (wheels != null)
            {
                float wheelAverage = GetWheelAverage();

                for (int i = 0; i < wheels.Count; i++)
                {
                    if (syncArmAngles)
                    {
                        wheels[i].SetArmAngle(Mathf.Lerp(wheels[i].ArmAngle, wheelAverage, armSyncStrength));
                    }

                    wheels[i].UpdateWheel(transform, angleSpeed);
                    wheels[i].UpdateArmPosition(transform);
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
