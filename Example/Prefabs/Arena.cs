using CoreDev.Sequencing;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private Vector2 weaponSpawnIntervalMinMaxSecs = new Vector2(1.0f, 5.0f);
    [SerializeField] private int numWeaponsPerSpawn = 3;

    private float weaponSpawnCountDown = 0.0f;


//*====================
//* UNITY
//*====================
    private void Awake()
    {
        this.RefreshCountDown();
        UniversalTimer.RegisterForTimeElapsed(this.TimeElapsed);
    }


//*====================
//* UniversalTimer
//*====================
    private void TimeElapsed(float deltaTime, float unscaledDeltaTime, int executionOrder)
    {
        weaponSpawnCountDown -= deltaTime;
        if (weaponSpawnCountDown <= 0.0f)
        {
            for (int i = 0; i < numWeaponsPerSpawn; i++)
            {
                float x = UnityEngine.Random.Range(-10.0f, 10.0f);
                float z = UnityEngine.Random.Range(-10.0f, 10.0f);
                new WeaponDO(new Vector3(x, 0.0f, z));
            }

            this.RefreshCountDown();
        }
    }


//*====================
//* PRIVATE
//*====================
    private void RefreshCountDown()
    {
        this.weaponSpawnCountDown = UnityEngine.Random.Range(weaponSpawnIntervalMinMaxSecs.x, weaponSpawnIntervalMinMaxSecs.y);
    }

}