using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace WheelchairAnim
{
    [System.Serializable]
    public class Wheel
    {
        [Header("Wheel")]
        [SerializeField]
        private Transform wheel;

        [Header("Arm Target")]
        [SerializeField]
        private Transform armTarget;
        [SerializeField]
        private float minAngle = 0;
        [SerializeField]
        private float maxAngle = 45;
        [SerializeField]
        private float timeToAdjustArm = 1.0f;
        [SerializeField]
        private float yAdjust = 1.0f;
        [SerializeField]
        private AnimationCurve armAdjustCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private float startingArmAngle = 0.0f;

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private float armAngle = 0;

        private Vector3 prevPosition;
        private Vector3 handOffset = Vector3.zero;

        private bool adjustingArmPosition = false;
        private float armAdjustTime = 0.0f;
        private Vector3 startArmAdjustPosition;
        private Vector3 targetArmPosition;

        public void Init()
        {
            prevPosition = wheel.position;
            targetArmPosition = armTarget.transform.position;
            handOffset = armTarget.position - wheel.position;
            armAngle = startingArmAngle;
        }

        public void StartAdjustArmPosition()
        {
            adjustingArmPosition = true;
            armAdjustTime = 0;
            startArmAdjustPosition = armTarget.transform.position;
        }

        public void UpdateWheel(Transform rootTransform, float angleSpeed)
        {
            //Figure how far we moved from last frame
            Vector3 leftPositionDelta = wheel.position - prevPosition;
            float distanceMoved = leftPositionDelta.magnitude;
            if (distanceMoved != 0)
            {
                //Figure which direction we went
                float angleFacing = Vector3.Angle(rootTransform.forward, leftPositionDelta);
                distanceMoved = distanceMoved * (angleFacing > 90.0f ? -1.0f : 1.0f);
                float angleDelta = distanceMoved * angleSpeed;

                //Rotate the wheel
                wheel.Rotate(Vector3.right, distanceMoved * angleSpeed);

                //Figure arm position and readjust when outside of bounds
                armAngle += angleDelta;
                if (armAngle > maxAngle)
                {
                    float diff = armAngle - maxAngle;
                    armAngle = minAngle + diff;
                    StartAdjustArmPosition();
                }
                if (armAngle < minAngle)
                {
                    float diff = minAngle - armAngle;
                    armAngle = maxAngle - diff;
                    StartAdjustArmPosition();
                }

                //Set the position of the arm relative to the wheel and rootTransform
                Vector3 offset = Quaternion.AngleAxis(armAngle, wheel.right) * rootTransform.TransformDirection(handOffset);
                targetArmPosition = wheel.position + offset;

                prevPosition = wheel.position;
            }
        }

        public void UpdateArmPosition()
        {
            if (adjustingArmPosition)
            {
                //Smooth the arm position to the target
                armAdjustTime += Time.deltaTime;
                float animSample = armAdjustCurve.Evaluate(armAdjustTime / timeToAdjustArm);

                //Bop it up a lil
                Vector3 sineAdjust = Vector3.up * Mathf.Sin(animSample * Mathf.PI) * yAdjust;
                armTarget.transform.position = Vector3.Lerp(startArmAdjustPosition, targetArmPosition, animSample) + sineAdjust;

                adjustingArmPosition = (armAdjustTime < timeToAdjustArm);
            }
            else
            {
                armTarget.transform.position = targetArmPosition;
            }
        }

        public void DrawGizmos()
        {
            if (armTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(armTarget.transform.position, 0.05f);
            }

            if (wheel != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(wheel.position, armTarget.position);
            }
        }
    }
}
