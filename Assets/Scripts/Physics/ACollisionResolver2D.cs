using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ForcesManager))]
    public abstract class ACollisionResolver2D : MonoBehaviour
    {
        private ForcesManager _forces;
        protected ForcesManager Forces
        {
            get
            {
                if (this._forces == null)
                    this._forces = this.GetComponent<ForcesManager>();
                return this._forces;
            }
        }

        /// <summary>
        /// Resolve a collision by pushing the current object out of the collider
        /// </summary>
        public void PushOutResolution(GDS.Physics.Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (!contact.penetration.HasValue)
                    continue;
                // Push object out of the collider
                GDS.Maths.Vector2 offset = contact.normal * contact.penetration.Value;
                this.transform.position = new UnityEngine.Vector3(
                    this.transform.position.x + offset.x,
                    this.transform.position.y + offset.y,
                    this.transform.position.z
                );
            }
        }

        public abstract void Resolve(GDS.Physics.Collision2D collision);
    }
}
