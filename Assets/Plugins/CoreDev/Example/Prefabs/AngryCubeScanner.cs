using System.Collections.ObjectModel;
using CoreDev.Examples;
using CoreDev.Framework;
using CoreDev.Sequencing;
using UnityEngine;

public class AngryCubeScanner : MonoBehaviour, ISpawnee
{
    private AngryCubeDO angryCubeDO;
    private ReadOnlyCollection<IDataObject> angryCubeDOs;


//*====================
//* UNITY
//*====================
    protected virtual void OnDestroy()
    {
        this.UnbindDO(this.angryCubeDO);
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
            this.angryCubeDOs = DataObjectMasterRepository.GetDataObjects<AngryCubeDO>();

            DataObjectMasterRepository.RegisterForDisposing(OnDataObjectDisposed);
            UniversalTimer.RegisterForTimeElapsed(Scan);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject as AngryCubeDO)
        {
            DataObjectMasterRepository.UnregisterFromDisposing(OnDataObjectDisposed);
            UniversalTimer.UnregisterFromTimeElapsed(Scan);

            this.angryCubeDO = null;
        }
    }


//*====================
//* CALLBACKS
//*====================
    private void OnDataObjectDisposed(IDataObject obj)
    {
        if (obj is AngryCubeDO)
        {
            AngryCubeDO angryCubeDO = obj as AngryCubeDO;
            this.angryCubeDO?.weaponizedEnemiesNearby.Remove(angryCubeDO);
        }
    }



//*====================
//* UniversalTimer
//*====================
    private void Scan(float deltaTime, float unscaledDeltaTime, int executionOrder)
    {
        foreach (AngryCubeDO otherCubeDO in angryCubeDOs)
        {
            if (IsOtherTeamAndWeaponized(otherCubeDO))
            {
                if (Vector3.Distance(this.angryCubeDO.pos_World.Value, otherCubeDO.pos_World.Value) <= this.angryCubeDO.scanRadius.Value)
                {
                    if (this.angryCubeDO.weaponizedEnemiesNearby.Contains(otherCubeDO) == false)
                    {
                        this.angryCubeDO.weaponizedEnemiesNearby.Add(otherCubeDO);
                    }
                    continue;
                }
            }
            this.angryCubeDO.weaponizedEnemiesNearby.Remove(otherCubeDO);
        }
    }

    private bool IsOtherTeamAndWeaponized(AngryCubeDO otherCubeDO)
    {
        bool isOtherTeam = otherCubeDO.teamId.Value != this.angryCubeDO.teamId.Value;
        bool isWeaponized = otherCubeDO.hasWeapon.Value;
        bool isOtherTeamAndWeaponized = isOtherTeam && isWeaponized;

        return isOtherTeamAndWeaponized;
    }
}