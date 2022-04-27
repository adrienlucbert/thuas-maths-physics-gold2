using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(Renderer))]
    public class CircleCollider : ACollider2D
    {
        private Renderer _renderer;
        public GDS.Maths.Vector2 center
            => new GDS.Maths.Vector2(this.transform.position.x, this.transform.position.y);
        public float radius
        {
            get
            {
                GDS.Maths.Vector3 extents = (GDS.Maths.Vector3)this.GetComponent<Renderer>().bounds.extents;
                return Mathf.Max(extents.x, extents.y);
            }
        }

        private void Awake()
        {
            this._renderer = this.GetComponent<Renderer>();
        }

        public override float GetArea()
        {
            // Area of a circle = PI * r^2
            return Mathf.PI * this.radius * this.radius;
        }

        public override GDS.Physics.ContactPoint2D[] GetContactPoints(ACollider2D collider)
        {
            if (collider.GetType() == typeof(CircleCollider))
            {
                CircleCollider circleCollider = collider as CircleCollider;
                float distance = this.center.Distance(circleCollider.center);
                if (distance <= this.radius + circleCollider.radius)
                {
                    float penetration = this.radius - (distance - circleCollider.radius);
                    GDS.Maths.Vector2 normal = (this.center - circleCollider.center).normalized;
                    GDS.Maths.Vector2 contact = this.center - normal * this.radius;
                    return new ContactPoint2D[] { new ContactPoint2D(contact, normal, penetration) };
                }
                return new ContactPoint2D[] { };
            }
            if (collider.GetType() == typeof(LineCollider))
            {
                LineCollider lineCollider = collider as LineCollider;
                // Get projection of the circle center on the line
                GDS.Maths.Vector2 projection = lineCollider.origin + (this.center - lineCollider.origin).Project(lineCollider.direction);
                GDS.Maths.Vector2 normal = this.center - projection;
                float distance = normal.magnitude;
                if (distance <= this.radius)
                {
                    normal = normal.normalized;
                    float penetration = this.radius - distance;
                    GDS.Maths.Vector2 contact = projection - normal * penetration;
                    penetration = contact.Distance(projection);
                    return new ContactPoint2D[]
                    {
                        new ContactPoint2D(contact, normal, penetration)
                    };
                }
                return new ContactPoint2D[] { };
            }
            throw new System.Exception($"Unsupported collision type between {typeof(CircleCollider)} and {collider.GetType()}");
        }
    }
}
