using System.Collections.ObjectModel;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;


namespace CoreDev.Examples
{
    public class AngryCubeFindTarget : MonoBehaviour, ISpawnee
    {
        [SerializeField] private float targetEvaluationIntervalSecs = 1.0f;
        private AngryCubeDO angryCubeDO;


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

                UniversalTimer.UnscheduleCallback(FindNearestTarget);
            }
        }


//*====================
//* CALLBACKS - AngryCube
//*====================
        private void OnHasWeaponChanged(ObservableVar<bool> oHasWeapon)
        {
            if (oHasWeapon.Value == true)
            {
                this.FindNearestTarget(null);
            }
        }


//*====================
//* PRIVATE
//*====================
        private void FindNearestTarget(object[] obj)
        {
            if(this.angryCubeDO == null) { return; }

            ReadOnlyCollection<IDataObject> allCubes = DataObjectMasterRepository.GetDataObjects<AngryCubeDO>();

            float nearestEnemyDistance = float.PositiveInfinity;
            Vector3 myPos_World = this.angryCubeDO.pos_World.Value;
            AngryCubeDO nearestTarget = null;
            
            foreach (IDataObject dataObject in allCubes)
            {
                AngryCubeDO potentialTarget = dataObject as AngryCubeDO;
                
                if(IsEnemy(potentialTarget))
                {
                    Vector3 enemyPos_World = potentialTarget.pos_World.Value;
                    float distanceToEnemy = Vector3.Distance(myPos_World, enemyPos_World);
                    if(distanceToEnemy < nearestEnemyDistance)
                    {
                        nearestTarget = potentialTarget;
                        nearestEnemyDistance = distanceToEnemy;
                    }
                }
            }

            this.angryCubeDO.targetCube.Value = nearestTarget;
            UniversalTimer.ScheduleCallback(FindNearestTarget, targetEvaluationIntervalSecs);
        }

        private bool IsEnemy(AngryCubeDO potentialTarget)
        {
            int myTeamId = this.angryCubeDO.teamId.Value;
            int otherTeamId = potentialTarget.teamId.Value;
            bool isEnemy = myTeamId != otherTeamId;
            
            return isEnemy;
        }
    }
}