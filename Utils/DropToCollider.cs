using UnityEngine;

namespace CoreDev.Utils
{
    public class DropToCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask colliderLayerMask;

        [ContextMenu("Drop")]
        public void Drop()
        {
            RaycastHit hitInfo;

            bool raycastHit = Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, float.PositiveInfinity, colliderLayerMask);

            if (raycastHit)
            {
                this.transform.position = hitInfo.point;
            }
        }
    }
}