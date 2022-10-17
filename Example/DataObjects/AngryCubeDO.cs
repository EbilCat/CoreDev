using System;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Examples
{
    [Serializable]
    public class AngryCubeDO : IDataObject
    {
        
        public OInt teamId;
        public OUInt id;

        public OVector3 pos_World;
        public OQuaternion rot_World;
        public OVector3 scale_Local;
        public OVector3 forward_World;

        public OBool isAlive;

        public OFloat moveSpeedUnitPerSec;
        public OVector3 moveToLocation_World;

        public OBool hasWeapon;

        public OAngryCube targetCube;

        public OList<AngryCubeDO> weaponizedEnemiesNearby;
        public OFloat scanRadius;
        

        private static uint runningId = 0;

        public AngryCubeDO(int teamId, Vector3 pos_World, Quaternion rot_World, int healthMax, float moveSpeedUnitPerSec)
        {
            runningId++;
            this.teamId = new OInt(teamId, this);
            this.id = new OUInt(runningId, this);

            this.pos_World = new OVector3(pos_World, this);
            this.rot_World = new OQuaternion(rot_World, this);
            this.scale_Local = new OVector3(Vector3.one, this);
            this.forward_World = new OVector3(Vector3.forward, this);

            this.isAlive = new OBool(true, this);
            this.moveSpeedUnitPerSec = new OFloat(moveSpeedUnitPerSec, this);
            this.moveToLocation_World = new OVector3(default(Vector3), this);
            this.hasWeapon = new OBool(false, this);
            this.targetCube = new OAngryCube(null, this);
            
            this.weaponizedEnemiesNearby = new OList<AngryCubeDO>(this);
            this.scanRadius = new OFloat(5.0f, this);

            DataObjectMasterRepository.RegisterDataObject(this);
        }

        public override string ToString()
        {
            string str = $"Cube {this.id.Value}";
            return str;
        }


        public event Action<IDataObject> disposing;
        public void Dispose()
        {
            disposing?.Invoke(this);
        }
    }
}