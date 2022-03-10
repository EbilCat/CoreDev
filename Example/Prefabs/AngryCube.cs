using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Examples
{
    public class AngryCube : MonoBehaviour, ISpawnee
    {
        private AngryCubeDO angryCubeDO;
        public AngryCubeDO AngryCubeDO
        {
            get { return angryCubeDO; }
        }


//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO)
            {
                UnbindDO(this.angryCubeDO);
                this.angryCubeDO = dataObject as AngryCubeDO;
                this.angryCubeDO.pos_World.RegisterForChanges(OnPos_WorldChanged);
                this.angryCubeDO.rot_World.RegisterForChanges(OnRot_WorldChanged);
                this.angryCubeDO.scale_Local.RegisterForChanges(OnScale_LocalChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject)
            {
                this.angryCubeDO?.pos_World.UnregisterFromChanges(OnPos_WorldChanged);
                this.angryCubeDO?.rot_World.UnregisterFromChanges(OnRot_WorldChanged);
                this.angryCubeDO?.scale_Local.UnregisterFromChanges(OnScale_LocalChanged);
                this.angryCubeDO = null;
            }
        }


//*====================
//* CALLBACKS
//*====================
        private void OnPos_WorldChanged(ObservableVar<Vector3> oPos_World)
        {
            this.transform.position = oPos_World.Value;
        }

        private void OnRot_WorldChanged(ObservableVar<Quaternion> oRot_World)
        {
            this.transform.rotation = oRot_World.Value;
            this.angryCubeDO.forward_World.Value = this.transform.forward;
        }
        
        private void OnScale_LocalChanged(ObservableVar<Vector3> oScale_Local)
        {
            this.transform.localScale = oScale_Local.Value;
        }
    }
}