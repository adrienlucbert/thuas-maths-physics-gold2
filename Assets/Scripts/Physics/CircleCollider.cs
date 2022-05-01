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

        public override bool GetContactPoints(ACollider2D collider, float step, out ContactPoint2D[] contacts, out float toi)
        {
            contacts = null;
            toi = 0f;

            if (collider.GetType() == typeof(CircleCollider))
            {
                CircleCollider rhs = collider as CircleCollider;

                // DCD pass
                var sc1 = new CollisionHelpers2D.StaticCircleInput { center = this.center, radius = this.radius };
                var sc2 = new CollisionHelpers2D.StaticCircleInput { center = rhs.center, radius = rhs.radius };
                if (CollisionHelpers2D.DCD.GetCircleCircleOverlap(sc1, sc2, out var overlap))
                {
                    contacts = new ContactPoint2D[]
                    {
                      new ContactPoint2D(overlap.position, overlap.normal, overlap.penetration)
                    };
                    return true;
                }

                // CCD pass
                if (!this.useCCD)
                    return false;
                Maths.Vector2 lhsSpeed = new Maths.Vector2(this.Forces.Speed.x, this.Forces.Speed.y);
                Maths.Vector2 rhsSpeed = new Maths.Vector2(rhs.Forces?.Speed.x ?? 0f, rhs.Forces?.Speed.y ?? 0f);
                var mc1 = new CollisionHelpers2D.MovingCircleInput { center = sc1.center, radius = sc1.radius, speed = lhsSpeed };
                var mc2 = new CollisionHelpers2D.MovingCircleInput { center = sc2.center, radius = sc2.radius, speed = rhsSpeed };
                if (!CollisionHelpers2D.CCD.GetCircleCircleIntersection(mc1, mc2, out var collision, step))
                    return false;

                contacts = new ContactPoint2D[]
                {
                  new ContactPoint2D(collision.position + collision.normal * this.radius, collision.normal)
                };
                toi = collision.toi;
                return true;
            }
            if (collider.GetType() == typeof(LineCollider))
            {
                LineCollider rhs = collider as LineCollider;

                // DCD pass
                var sc = new CollisionHelpers2D.StaticCircleInput { center = this.center, radius = this.radius };
                var line = new CollisionHelpers2D.LineInput { origin = rhs.origin, direction = rhs.direction };
                if (CollisionHelpers2D.DCD.GetCircleLineOverlap(sc, line, out var overlap))
                {
                    contacts = new ContactPoint2D[]
                    {
                      new ContactPoint2D(overlap.position, overlap.normal, overlap.penetration)
                    };
                    return true;
                }

                // CCD pass
                if (!this.useCCD)
                    return false;
                Maths.Vector2 lhsSpeed = new Maths.Vector2(this.Forces.Speed.x, this.Forces.Speed.y);
                var mc = new CollisionHelpers2D.MovingCircleInput { center = this.center, radius = this.radius, speed = lhsSpeed };
                if (!CollisionHelpers2D.CCD.GetCircleLineIntersection(mc, line, out var collision, step))
                    return false;

                GDS.Maths.Vector2 contact = collision.position - (collision.normal * this.radius);
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
