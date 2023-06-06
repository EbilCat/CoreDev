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

        public void ScreenPointToLocalPointInRectangle_HS(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 convertedPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out convertedPoint);
        }
        protected override void Awake()
        {
            this.Claim(this.moveSpeed);
            this.Claim(this.turnSpeed);
            base.Awake();
        }
    }
}
