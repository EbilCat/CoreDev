using CoreDev.Sequencing;
using UnityEngine;

namespace TimeElapsedTester
{
    public class MovingCube : MonoBehaviour, IHasInitHandler
    {
        [SerializeField] private float xMoveSpeed = 0.5f;
        [SerializeField] private float zMoveSpeed = 0.5f;
        private int xMoveDir = 1;
        [SerializeField] private float maxDisplacement = 1.0f;
        private int zMoveDir = 1;


//*====================
//* UNITY
//*====================
        private void Awake() 
        { 
            UniversalTimer.RegisterForInit(this); 
        }
        public void Init()
        {
            UniversalTimer.RegisterForTimeElapsed(MoveCube);
        }

        private void MoveCube(float deltaTime, float unscaledDeltaTime, int executionOrder)
        {
            Vector3 cubePosition = this.transform.position;
            
            cubePosition.x += Random.Range(0.0f, xMoveSpeed) * xMoveDir * Time.deltaTime;
            cubePosition.x = Mathf.Clamp(cubePosition.x, -maxDisplacement, maxDisplacement);
            if(Mathf.Abs(cubePosition.x) == maxDisplacement) { xMoveDir *= -1; }
            
            cubePosition.z += Random.Range(0.0f, zMoveSpeed) * zMoveDir * Time.deltaTime;
            cubePosition.z = Mathf.Clamp(cubePosition.z, -maxDisplacement, maxDisplacement);
            if(Mathf.Abs(cubePosition.z) == maxDisplacement) { zMoveDir *= -1; }
            
            this.transform.position = cubePosition;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                UniversalTimer.UnregisterFromTimeElapsed(MoveCube);
                UniversalTimer.ScheduleCallback(StartCubeMoving, 2.0f);
            }
        }


//*====================
//* PRIVATE
//*====================
        private void StartCubeMoving(object[] obj)
        {
            UniversalTimer.RegisterForTimeElapsed(MoveCube);
        }
    }
}