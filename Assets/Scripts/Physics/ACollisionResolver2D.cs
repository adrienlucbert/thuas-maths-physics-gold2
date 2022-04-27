using UnityEngine;

namespace GDS.Physics
{
    public abstract class ACollisionResolver2D : MonoBehaviour
    {
        public virtual void Resolve(GDS.Physics.Collision2D collision)
        {
            collision.DrawGizmos();
        }
    }
}
