using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ForcesManager))]
    public abstract class ACollisionResolver2D : MonoBehaviour
    {
        protected ForcesManager Forces;

        protected void Awake()
        {
            this.Forces = this.GetComponent<ForcesManager>();
        }

        public virtual void Resolve(GDS.Physics.Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                GDS.Maths.Vector2 offset = contact.normal * contact.penetration;
                this.transform.position = new UnityEngine.Vector3(
                    this.transform.position.x + offset.x,
                    this.transform.position.y + offset.y,
                    this.transform.position.z
                );
            }
        }
    }
}
