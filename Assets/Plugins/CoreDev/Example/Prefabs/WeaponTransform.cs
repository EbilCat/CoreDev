using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class WeaponTransform : MonoBehaviour, ISpawnee
{
    private WeaponDO weaponDO;


//*====================
//* BINDING
//*====================
    public void BindDO(IDataObject dataObject)
    {
        if(dataObject is WeaponDO)
        {
            UnbindDO(this.weaponDO);
            this.weaponDO = dataObject as WeaponDO;
            this.weaponDO.pos_World.RegisterForChanges(OnPos_WorldChanged);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if(dataObject is WeaponDO && this.weaponDO == dataObject)
        {
            this.weaponDO?.pos_World.UnregisterFromChanges(OnPos_WorldChanged);
            this.weaponDO = null;
        }
    }

    private void OnPos_WorldChanged(ObservableVar<Vector3> oPos_World)
    {
        this.transform.position = oPos_World.Value;
    }
}