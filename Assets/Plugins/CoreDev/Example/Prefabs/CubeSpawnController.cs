using UnityEngine;


namespace CoreDev.Examples
{
    public class CubeSpawnController : MonoBehaviour
    {
        [SerializeField] private int teamId;
        [SerializeField] private int health;
        [SerializeField] float spawnIntervalSecs = 1.0f;
        [SerializeField] float moveSpeedUnitPerSec = 1.0f;
        private float countDownSecs = 0.0f;


        private void Update()
        {
            if (countDownSecs <= 0.0f)
            {
                AngryCubeDO angryCubeDO = new AngryCubeDO(teamId, this.transform.position, this.transform.rotation, health, moveSpeedUnitPerSec);
                angryCubeDO.moveSpeedUnitPerSec.AddModerator(RejectNegativeSpeed);
                countDownSecs = spawnIntervalSecs;
            }
            countDownSecs -= Time.deltaTime;
        }

        private bool RejectNegativeSpeed(ref float incomingValue)
        {
            if(incomingValue < 0.0f)
            {
                return false;
            }
            return true;
        }
    }
}