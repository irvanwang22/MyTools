using System;
using Unity.Mathematics;
using UnityEngine;

namespace MyTools
{
    public class SpringArm : MonoBehaviour
    {
        public GameObject target;
        public float armLength = 10f;
        public float smoothTime = 0.3f; // time to reach the target
        public float maxSpeed = 10f;

        private Vector3 targetPos;
        private Vector3 velocity = Vector3.zero; // SmoothDamp speed reference

        private void Start()
        {
            ResetPosition();
        }

        private void LateUpdate()
        {
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position,
                GetDesiredPosition(),
                ref velocity,
                smoothTime,
                maxSpeed
            );
        }

        public void ForceSetTargetPosition(Vector3 position)
        {
            target = null;
            targetPos = position;
        }

        public void ResetPosition()
        {
            this.transform.position = GetDesiredPosition();
        }

        public void Move(Vector3 delta)
        {
            this.targetPos += (target == null) ? 
                delta : Vector3.zero;
        }

        public void MoveTo(Vector3 pos)
        {
            this.targetPos = pos;
        }

        public void Rotate(Quaternion deltaRot)
        {
            this.transform.rotation = deltaRot * this.transform.rotation;
        }

        public void Rotate(Vector3 eulerAngles, Space relativeTo = Space.Self)
        {
            this.transform.Rotate(eulerAngles, relativeTo);
        }

        private Vector3 GetTargetPosition()
        {
            if (target != null)
            {
                targetPos = target.transform.position;
            }
            return targetPos;
        }

        private Vector3 GetDesiredPosition()
        {
            return GetTargetPosition() - this.transform.forward * armLength;
        }
    }
}