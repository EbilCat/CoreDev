using CoreDev.Sequencing;
using UnityEngine;

namespace TimeElapsedTester
{
    public class Smiley : MonoBehaviour
    {
        public Transform followTransform;
        private bool usingUniversalTimer = false;
        private int executionOrder;
        private Vector3 posAtStartOfFrame;


//*====================
//* UNITY
//*====================
        private void Update()
        {
            if (usingUniversalTimer == false)
            {
                this.FollowCube();
            }
        }


//*====================
//* UniversalTimer
//*====================
        public void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            if (executionOrder == Constants.StartOfFrame + this.executionOrder) { this.TakeSnapShot(); }
            if (executionOrder == Constants.EndOfFrame + this.executionOrder)
            {
                this.FollowCube();
                this.CalculateMovementDelta();
            }
        }

//*====================
//* PUBLIC
//*====================
        public void Init(Transform followTransform, bool usingUniversalTimer, int executionOrder)
        {
            this.followTransform = followTransform;
            this.usingUniversalTimer = usingUniversalTimer;
            this.executionOrder = executionOrder;

            if (usingUniversalTimer)
            {
                UniversalTimer.RegisterForTimeElapsed(TimeElapsed, Constants.StartOfFrame + this.executionOrder);
                UniversalTimer.RegisterForTimeElapsed(TimeElapsed, Constants.EndOfFrame + this.executionOrder);
            }
        }

//*====================
//* PRIVATE
//*====================
        private void TakeSnapShot()
        {
            this.posAtStartOfFrame = this.followTransform.position;
        }

        private void FollowCube()
        {
            Vector3 pos = this.transform.position;
            pos.x = followTransform.position.x;
            pos.y = followTransform.position.y + 2f;
            pos.z = followTransform.position.z;
            this.transform.position = pos;
        }
        private void CalculateMovementDelta()
        {
            Vector3 movementDelta = this.followTransform.position - posAtStartOfFrame;
            // Debug.Log($"{this.name} Movement Delta: {movementDelta.ToString("0.00")}");
        }
    }
}