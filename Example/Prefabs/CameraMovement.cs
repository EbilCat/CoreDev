using CoreDev.Framework;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Examples
{
    public class CameraMovement : MonoBehaviour, ISpawnee
    {
        private ICameraDO cameraDO;


//*====================
//* UNITY
//*====================
        protected virtual void OnDestroy()
        {
            this.UnbindDO(this.cameraDO);
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is ICameraDO)
            {
                UnbindDO(this.cameraDO);
                this.cameraDO = dataObject as ICameraDO;
                UniversalTimer.RegisterForTimeElapsed(MoveCamera);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is ICameraDO && this.cameraDO == dataObject as ICameraDO)
            {
                UniversalTimer.UnregisterFromTimeElapsed(MoveCamera);
                this.cameraDO = null;
            }
        }


//*====================
//* CALLBACKS - UniversalTimer
//*====================
        private void MoveCamera(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            if(Input.GetKey(KeyCode.W))
            {
                this.cameraDO.Pos_World += Vector3.ProjectOnPlane(this.cameraDO.Forward_World, Vector3.up).normalized * this.cameraDO.MoveSpeed.Value * deltaTime;
            }
            
            if(Input.GetKey(KeyCode.S))
            {
                this.cameraDO.Pos_World -= Vector3.ProjectOnPlane(this.cameraDO.Forward_World, Vector3.up).normalized * this.cameraDO.MoveSpeed.Value * deltaTime;
            }
            
            if(Input.GetKey(KeyCode.D))
            {
                this.cameraDO.Rot_World = Quaternion.Euler(0.0f, this.cameraDO.TurnSpeed.Value * deltaTime, 0.0f) * this.cameraDO.Rot_World;
            }

            if(Input.GetKey(KeyCode.A))
            {
                this.cameraDO.Rot_World = Quaternion.Euler(0.0f, -this.cameraDO.TurnSpeed.Value * deltaTime, 0.0f) * this.cameraDO.Rot_World;
            }
        }
    }
}
