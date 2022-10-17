using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Framework.Testing
{

    public class CubeRotationDO : MonoBehaviourDO
    {
        [SerializeField] private OFloat rotationSpeed = new OFloat(10.0f);
        public OFloat RotationSpeed => rotationSpeed;


        protected override void Awake()
        {
            base.Awake();
            this.Claim(rotationSpeed);
        }
    }
}