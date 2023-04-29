using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace WheelchairAnim
{
    [System.Serializable]
    public class Wheel
    {
        #region Fields

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
        [SerializeField, Range(0, 1)]
        private float movementBlendStrength = 0.1f;


        [Header("Arm Adjusting")]
        [SerializeField]
        private float timeToAdjustArm = 1.0f;
        [SerializeField]
        private float yAdjust = 1.0f;
        [SerializeField]
        private AnimationCurve armAdjustCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private float startingArmAngle = 0.0f;


        [Header("Flailing")]
        [SerializeField]
        private float maxSpeedToFlail = 1.0f;
        [SerializeField, Range(0, 1)]
        private float flailArmMoveStrength = 0.1f;
        [SerializeField]
        private Vector3 forwardFlailOffset;
        [SerializeField]
        private Vector3 backwardFlailOffset;

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private float armAngle = 0;
        [SerializeField, ReadOnly]
        private bool isFlailing = false;
        [SerializeField]
        private float amountMoved = 0;

        #endregion

        public float ArmAngle => armAngle;

        private Vector3 prevPosition;
        private Vector3 handOffset = Vector3.zero;

        private bool adjustingArmPosition = false;
        private float armAdjustTime = 0.0f;
        private float startArmAdjustAngle;
        private Vector3 targetFlailPosition;

        public void Init()
        {
            prevPosition = wheel.position;
            handOffset = armTarget.position - wheel.position;
            armAngle = startingArmAngle;
        }

        public void StartAdjustArmPosition()
        {
            adjustingArmPosition = true;
            armAdjustTime = 0;
            startArmAdjustAngle = armAngle;
        }

        public void SetArmAngle(float armAngle)
        {
            this.armAngle = armAngle;
        }

        public void UpdateWheel(Transform rootTransform, float angleSpeed)
        {
            //Figure how far we moved from last frame
            Vector3 lastPositionDelta = wheel.position - prevPosition;
            float distanceMoved = lastPositionDelta.magnitude;

            amountMoved = distanceMoved;

            //Figure which direction we went
            float angleFacing = Vector3.Angle(rootTransform.forward, lastPositionDelta);
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

            prevPosition = wheel.position;

            isFlailing = Mathf.Abs(distanceMoved) > maxSpeedToFlail * Time.deltaTime;

            //Moving forward -> flailing backward
            Vector3 flailOffset = Mathf.Sign(distanceMoved) > 0 ? backwardFlailOffset : forwardFlailOffset;
            targetFlailPosition = wheel.transform.position + rootTransform.TransformDirection(flailOffset);
        }

        private Vector3 GetAnglePosition(float angle, Transform rootTransform)
        {
            //Set the position of the arm relative to the wheel and rootTransform
            Vector3 adjustedOffset = Quaternion.AngleAxis(angle, wheel.right) * rootTransform.TransformDirection(handOffset);
            return wheel.position + adjustedOffset;
        }

        public void UpdateArmPosition(Transform rootTransform)
        {
            Vector3 targetArmPosition = GetAnglePosition(armAngle, rootTransform);

            if (isFlailing)
            {
                targetArmPosition = targetFlailPosition;
            }
            else if (adjustingArmPosition)
            {
                //Smooth the arm position to the target
                armAdjustTime += Time.deltaTime;
                float animSample = armAdjustCurve.Evaluate(armAdjustTime / timeToAdjustArm);

                //Bop it up a lil
                Vector3 sineAdjust = Vector3.up * Mathf.Sin(animSample * Mathf.PI) * yAdjust;
                Vector3 startArmPosition = GetAnglePosition(startArmAdjustAngle, rootTransform);

                targetArmPosition = Vector3.Lerp(startArmPosition, targetArmPosition, animSample) + sineAdjust;

                adjustingArmPosition = (armAdjustTime < timeToAdjustArm);
            }

            armTarget.transform.position = Vector3.Lerp(armTarget.transform.position, targetArmPosition, movementBlendStrength);
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
