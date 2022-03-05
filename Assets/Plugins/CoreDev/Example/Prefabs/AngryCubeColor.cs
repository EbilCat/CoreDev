using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Examples
{
    public class AngryCubeColor : MonoBehaviour, ISpawnee
    {
        private AngryCubeDO angryCubeDO;
        private GameSettingsDO gameSettingsDO;
        private Material material;


        //*====================
        //* BINDING
        //*====================
        public void BindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO)
            {
                this.material = this.GetComponent<MeshRenderer>().material;
                this.gameSettingsDO = GameSettingsDO.Instance;
                
                UnbindDO(this.angryCubeDO);
                this.angryCubeDO = dataObject as AngryCubeDO;
                this.angryCubeDO.teamId.RegisterForChanges(OnTeamIdChanged);
            }
        }

        public void UnbindDO(IDataObject dataObject)
        {
            if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject)
            {
                this.angryCubeDO?.teamId.UnregisterFromChanges(OnTeamIdChanged);
                this.angryCubeDO = null;
            }
        }


//*====================
//* CALLBACKS - AngryCubeDO
//*====================
        private void OnTeamIdChanged(ObservableVar<int> oTeamId)
        {
            Color color = gameSettingsDO.teamColors.Value[oTeamId.Value - 1];
            this.material.color = color;
        }
    }
}