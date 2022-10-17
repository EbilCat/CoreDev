
using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class WeaponDO : IDataObject
{
    public OVector3 pos_World;

    public WeaponDO(Vector3 pos_World)
    {
        this.pos_World = new OVector3(pos_World, this);

        DataObjectMasterRepository.RegisterDataObject(this);
    }

        
//*====================
//* IDataObject
//*====================
        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
}