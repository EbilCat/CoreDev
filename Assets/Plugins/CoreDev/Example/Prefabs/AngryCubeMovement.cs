using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Examples
{
    public class AngryCubeMovement : MonoBehaviour, ISpawnee
    {
        [SerializeField] private Vector2 evalMoveLocIntervalMinMax = new Vector2(3.0f, 10.0f);
        private float chooseMoveLocationCountDownSecs = 0.0f;
        private AngryCubeDO angryCubeDO;
        private AngryCubeDO targetCubeDO;


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO)
            {
                UnbindDO(this.angryCubeDO);
                this.angryCubeDO = dataObject as AngryCubeDO;

                this.angryCubeDO.targetCube.RegisterForChanges(OnTargetCubeChanged);

                UniversalTimer.RegisterForTimeElapsed(TimeElapsed);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject)
            {
                this.angryCubeDO?.targetCube.UnregisterFromChanges(OnTargetCubeChanged);
                this.angryCubeDO = null;

                UniversalTimer.UnregisterFromTimeElapsed(TimeElapsed);
            }
        }


//*====================
//* CALLBACKS - UniversalTimer
//*====================
        public void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            if (this.targetCubeDO == null)
            {
                Wander(deltaTime);
            }
            else
            {
                ChaseTarget();
            }
            PerformMovement(deltaTime);
        }


//*====================
//* CALLBACKS - AngryCubeDO
//*====================
        private void OnTargetCubeChanged(ObservableVar<AngryCubeDO> oTargetCube)
        {
            DataObjectMasterRepository.RegisterForDisposing(OnDataObjectDisposing);
            this.targetCubeDO = oTargetCube.Value;
        }

        private void OnDataObjectDisposing(IDataObject dataObject)
        {
            if (this.targetCubeDO == dataObject)
            {
                this.targetCubeDO = null;
            }
        }


//*====================
//* PRIVATE
//*====================
        private void ChaseTarget()
        {
            Vector3 targetPos_World = this.targetCubeDO.pos_World.Value;
            this.angryCubeDO.moveToLocation_World.Value = targetPos_World;
        }

        private void Wander(float timeElapsedSecs)
        {
            if (chooseMoveLocationCountDownSecs <= 0.0f)
            {
                float x = UnityEngine.Random.Range(-10.0f, 10.0f);
                float z = UnityEngine.Random.Range(-10.0f, 10.0f);

                this.angryCubeDO.moveToLocation_World.Value = new Vector3(x, 0.0f, z);

                this.chooseMoveLocationCountDownSecs = UnityEngine.Random.Range(evalMoveLocIntervalMinMax.x, evalMoveLocIntervalMinMax.y);
            }
            chooseMoveLocationCountDownSecs -= timeElapsedSecs;
        }

        private void PerformMovement(float timeElapsedSecs)
        {
            Vector3 cubePos_World = this.angryCubeDO.pos_World.Value;
            Vector3 moveToLocation_World = this.angryCubeDO.moveToLocation_World.Value;
            Vector3 moveDir_World = (moveToLocation_World - cubePos_World).normalized;
            float moveSpeedUnitPerSec = this.angryCubeDO.moveSpeedUnitPerSec.Value;

            this.angryCubeDO.pos_World.Value += moveDir_World * timeElapsedSecs * moveSpeedUnitPerSec * ((this.angryCubeDO.hasWeapon.Value) ? 0.75f : 1.0f);
        }
    }
}