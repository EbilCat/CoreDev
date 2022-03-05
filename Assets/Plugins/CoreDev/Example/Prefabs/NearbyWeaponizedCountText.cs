using System;
using CoreDev.Examples;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;
using UnityEngine.UI;

public class NearbyWeaponizedCountText : MonoBehaviour, ISpawnee
{
    private AngryCubeDO angryCubeDO;
    private Camera cam;
    private Text text;


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
            this.cam = Camera.main;
            text = this.GetComponent<Text>();

            this.angryCubeDO.pos_World.RegisterForChanges(OnPos_WorldChanged);
            this.angryCubeDO.weaponizedEnemiesNearby.RegisterForElementAdded(OnWeaponizedEnemiesNearbyChanged);
            this.angryCubeDO.weaponizedEnemiesNearby.RegisterForElementRemoved(OnWeaponizedEnemiesNearbyChanged);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is AngryCubeDO && this.angryCubeDO == dataObject as AngryCubeDO)
        {
            this.angryCubeDO?.pos_World.UnregisterFromChanges(OnPos_WorldChanged);
            this.angryCubeDO?.weaponizedEnemiesNearby.UnregisterFromElementAdded(OnWeaponizedEnemiesNearbyChanged);
            this.angryCubeDO?.weaponizedEnemiesNearby.UnregisterFromElementAdded(OnWeaponizedEnemiesNearbyChanged);
            this.angryCubeDO = null;
        }
    }


    //*====================
    //* CALLBACKS
    //*====================
    private void OnPos_WorldChanged(ObservableVar<Vector3> obj)
    {
        Vector3 pos_ViewPort = this.cam.WorldToViewportPoint(obj.Value);
        RectTransform rectTransform = this.transform as RectTransform;
        rectTransform.anchorMin = pos_ViewPort;
        rectTransform.anchorMax = pos_ViewPort;
    }

    private void OnWeaponizedEnemiesNearbyChanged(OList<AngryCubeDO> list, AngryCubeDO element)
    {
        this.text.text = $"{this.angryCubeDO.weaponizedEnemiesNearby.Count}";
    }
}