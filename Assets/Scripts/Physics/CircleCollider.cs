using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(Renderer))]
    public class CircleCollider : ACollider2D
    {
        private Renderer __renderer;
        private Renderer _renderer
        {
            get
            {
                if (this.__renderer == null)
                    this.__renderer = this.GetComponent<Renderer>();
                return this.__renderer;
            }
        }
        public GDS.Maths.Vector2 center
            => new GDS.Maths.Vector2(this.transform.position.x, this.transform.position.y);
        public float radius
        {
            get
            {
                GDS.Maths.Vector3 extents = (GDS.Maths.Vector3)this._renderer.bounds.extents;
                return Mathf.Max(extents.x, extents.y);
            }
        }

        public override float GetArea()
        {
            // Area of a circle = PI * r^2
            return Mathf.PI * this.radius * this.radius;
        }

        public override bool GetContactPoints(ACollider2D collider, out ContactPoint2D[] contacts, out float toi)
        {
            contacts = null;
            toi = 0f;

            // Gizmos.color = Color.red;
            // Gizmos.DrawLine((UnityEngine.Vector2)this.center, (UnityEngine.Vector2)(this.center + new Maths.Vector2(this.Forces.Speed.x, this.Forces.Speed.y)));

            if (collider.GetType() == typeof(CircleCollider))
            {
                CircleCollider rhs = collider as CircleCollider;
                Maths.Vector2 lhsSpeed = new Maths.Vector2(this.Forces.Speed.x, this.Forces.Speed.y);
                Maths.Vector2 rhsSpeed = new Maths.Vector2(rhs.Forces?.Speed.x ?? 0f, rhs.Forces?.Speed.y ?? 0f);
                if (!CollisionHelpers2D.CCD.GetCircleCircleIntersection(
                      new CollisionHelpers2D.MovingCircleInput { center = this.center, radius = this.radius, speed = lhsSpeed },
                      new CollisionHelpers2D.MovingCircleInput { center = rhs.center, radius = rhs.radius, speed = rhsSpeed },
                      out var collision, Time.deltaTime))
                    return false;

                // Gizmos.color = Color.red;
                // Gizmos.DrawSphere((UnityEngine.Vector2)collision.position, 0.05f);
                // Gizmos.DrawLine((UnityEngine.Vector2)collision.position, (UnityEngine.Vector2)(collision.position + collision.normal));

                // Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
                // Gizmos.DrawSphere((UnityEngine.Vector2)(collision.position), 0.25f);
                // Gizmos.DrawSphere((UnityEngine.Vector2)(rhs.center + (rhsSpeed * collision.toi)), 0.25f);

                contacts = new ContactPoint2D[]
                {
                  new ContactPoint2D(collision.position, collision.normal)
                };
                toi = collision.toi;
                return true;
            }
            if (collider.GetType() == typeof(LineCollider))
            {
                LineCollider rhs = collider as LineCollider;
                Maths.Vector2 lhsSpeed = new Maths.Vector2(this.Forces.Speed.x, this.Forces.Speed.y);
                if (!CollisionHelpers2D.CCD.GetCircleLineIntersection(
                      new CollisionHelpers2D.MovingCircleInput { center = this.center, radius = this.radius, speed = lhsSpeed },
                      new CollisionHelpers2D.LineInput { origin = rhs.origin, direction = rhs.direction },
                      out var collision, Time.deltaTime))
                    return false;

                // Gizmos.color = Color.red;
                // Gizmos.DrawSphere((UnityEngine.Vector2)collision.position, 0.05f);

                GDS.Maths.Vector2 contact = collision.position - (collision.normal * this.radius);

                // Gizmos.color = Color.blue;
                // Gizmos.DrawLine((UnityEngine.Vector2)contact, (UnityEngine.Vector2)(contact + collision.normal));
                // Gizmos.DrawSphere((UnityEngine.Vector2)contact, 0.05f);

                contacts = new ContactPoint2D[]
                {
                  new ContactPoint2D(contact, collision.normal)
                };
                toi = collision.toi;
                return true;
            }
            throw new System.Exception($"Unsupported collision type between {typeof(CircleCollider)} and {collider.GetType()}");
        }
    }
}
