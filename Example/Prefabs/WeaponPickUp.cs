using CoreDev.Framework;
using UnityEngine;


namespace CoreDev.Examples
{
    public class WeaponPickUp : MonoBehaviour, ISpawnee
    {
        private WeaponDO weaponDO;
    
    
//*====================
//* UNITY
//*====================
    private void OnTriggerEnter(Collider other)
    {
        AngryCube angryCube = other.GetComponent<AngryCube>();
        if(angryCube != null)
        {
                AngryCubeDO angryCubeDO = angryCube.AngryCubeDO;
                Debug.Log($"Cube {angryCubeDO.id.Value} picked up weapon");
                angryCubeDO.hasWeapon.Value = true;
                this.weaponDO.Dispose();
        }
    }
    

//*====================
//* BINDING
//*====================
        public void BindDO(IDataObject dataObject)
        {
            if(dataObject is WeaponDO)
            {
                UnbindDO(this.weaponDO);
                this.weaponDO = dataObject as WeaponDO;
            }
        }
    
        public void UnbindDO(IDataObject dataObject)
        {
            if(dataObject is WeaponDO && this.weaponDO == dataObject)
            {
                this.weaponDO = null;
            }
        }
    }
}