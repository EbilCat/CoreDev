using UnityEngine;

namespace CoreDev.Utils
{
    public class DropToCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        [ContextMenu("Drop")]
        private void Drop()
        {
            RaycastHit hitInfo;

            bool raycastHit = Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, float.PositiveInfinity, layerMask);

            if (raycastHit)
            {
                this.transform.position = hitInfo.point;
            }
        }
    }
}