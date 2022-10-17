using CoreDev.Observable;
using UnityEngine;


namespace CoreDev.Framework.Testing
{
    public class CubeScaleDO : MonoBehaviourDO
    {
        [SerializeField] private OFloat scale_World = new OFloat(1.0f);
        public OFloat Scale_World => scale_World;

        protected override void Awake()
        {
            base.Awake();
            this.Claim(scale_World);
        }
    }
}