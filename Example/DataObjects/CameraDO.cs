using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

namespace CoreDev.Examples
{
    public interface ICameraDO : ITransform, IDataObject
    {
        OFloat MoveSpeed { get; }

        OFloat TurnSpeed { get; }
        
    }

    public class CameraDO : TransformDO, ICameraDO
    {
        [SerializeField] private OFloat moveSpeed = new OFloat(1.0f);
        public OFloat MoveSpeed => moveSpeed;
        
        [SerializeField] private OFloat turnSpeed = new OFloat(45.0f);
        public OFloat TurnSpeed => turnSpeed;

        protected override void Awake()
        {
            this.Claim(this.moveSpeed);
            this.Claim(this.turnSpeed);
            base.Awake();
        }
    }
}
