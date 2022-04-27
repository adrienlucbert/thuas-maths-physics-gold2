using UnityEngine;

namespace GDS.Physics
{
    public abstract class ACollider2D : MonoBehaviour
    {
        public bool isStatic = false;
        public float density = 1f;

        public abstract float GetArea();
        public float GetMass()
        {
            return this.GetArea() * this.density;
        }
        public abstract GDS.Physics.ContactPoint2D[] GetContactPoints(ACollider2D collider);
    }
}
