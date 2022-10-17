using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Framework.Testing
{
    public class CubeDriver : BaseSpawnee
    {
        private CubeRotationDO cubeRotationDO;
        private CubeScaleDO cubeScaleDO;


//*====================
//* BINDING
//*====================
        public override void BindDO(IDataObject dataObject)
        {
            base.BindDO(dataObject);
            if(this.AttemptDependencyBind(dataObject, ref cubeRotationDO))
            {
                this.BeginInit();
            }
        }

        protected override bool FulfillDependencies()
        {
            bool fulfilled = base.FulfillDependencies();
            fulfilled &= cubeRotationDO != null;
            fulfilled &= RetrieveDependency(ref cubeScaleDO);
            return fulfilled;
        }

        protected override void RegisterCallbacks()
        {
            this.cubeScaleDO.Scale_World.RegisterForChanges(OnScale_WorldChanged);
            UniversalTimer.RegisterForTimeElapsed(AnimateCube);
        }

        protected override void UnregisterCallbacks()
        {
            this.cubeScaleDO?.Scale_World.UnregisterFromChanges(OnScale_WorldChanged);
            UniversalTimer.UnregisterFromTimeElapsed(AnimateCube);
        }

        protected override void ClearDependencies(object obj = null)
        {
            base.ClearDependencies(obj);
            this.ClearDependency(ref cubeRotationDO);
            this.ClearDependency(ref cubeScaleDO);
        }


//*====================
//* CALLBACKS
//*====================
        private void AnimateCube(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            this.transform.rotation *= Quaternion.Euler(0.0f, cubeRotationDO.RotationSpeed.Value * deltaTime, 0.0f);
        }

        private void OnScale_WorldChanged(ObservableVar<float> obj)
        {
            this.transform.localScale = new Vector3(obj.Value, obj.Value, obj.Value);
        }
    }
}