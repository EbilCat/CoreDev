using System.Collections.Generic;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Examples
{

    public class AngryCubeKillBox : MonoBehaviour, ISpawnee
    {
        private AngryCubeDO angryCubeDO;

        private List<AngryCubeDO> cubesSlatedForDestruction = new List<AngryCubeDO>();


//*====================
//* UNITY
//*====================
        private void OnTriggerEnter(Collider other)
        {
            AngryCube otherCube = other.GetComponent<AngryCube>();
            if (otherCube != null)
            {
                AngryCubeDO otherCubeDO = otherCube.AngryCubeDO;

                if (this.angryCubeDO.teamId.Value != otherCubeDO.teamId.Value)
                {
                    Debug.Log($"Cube {this.angryCubeDO.id.Value} has defeated Cube {otherCubeDO.id.Value}");
                    otherCubeDO.isAlive.Value = false;
                    this.cubesSlatedForDestruction.Add(otherCubeDO);

                    UniversalTimer.ScheduleCallback(DestroyCube);//Do it next frame
                }
            }
        }

        private void DestroyCube(object[] obj)
        {
            foreach (AngryCubeDO cube in cubesSlatedForDestruction)
            {
                cube.Dispose();
            }
            cubesSlatedForDestruction.Clear();
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
                this.angryCubeDO.hasWeapon.RegisterForChanges(OnHasWeaponChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject)
            {
                this.angryCubeDO?.hasWeapon.UnregisterFromChanges(OnHasWeaponChanged);
                this.angryCubeDO = null;
            }
        }

        private void OnHasWeaponChanged(ObservableVar<bool> oHasWeapon)
        {
            bool hasWeapon = oHasWeapon.Value;
            this.gameObject.SetActive(hasWeapon);
        }
    }
}