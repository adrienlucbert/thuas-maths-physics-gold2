using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(Renderer))]
    public class QuadCollider : ACollider2D
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

        public Maths.Vector2 origin
          => new Maths.Vector2(this.transform.position.x, this.transform.position.y) - this.size / 2f;
        public Maths.Vector2 size
          => new Maths.Vector2(this._renderer.bounds.extents.x * 2f, this._renderer.bounds.extents.y * 2f);

        public override float GetArea()
        {
            Maths.Vector2 size = this.size;
            return size.x * size.y;
        }

        public override bool GetContactPoints(ACollider2D collider, float step, out ContactPoint2D[] contacts, out float toi)
        {
            contacts = null;
            toi = 0f;

            if (collider.GetType() == typeof(QuadCollider))
            {
                QuadCollider rhs = collider as QuadCollider;

                // DCD pass
                var sq1 = new CollisionHelpers2D.StaticQuadInput { origin = this.origin, size = this.size };
                var sq2 = new CollisionHelpers2D.StaticQuadInput { origin = rhs.origin, size = rhs.size };
                if (CollisionHelpers2D.DCD.GetQuadQuadOverlap(sq1, sq2, out var overlap))
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
                // TODO(alucbert): implement CCD for quad-quad intersection
                return false;
            }
            if (collider.GetType() == typeof(LineCollider))
            {
                LineCollider rhs = collider as LineCollider;

                // DCD pass
                var sq = new CollisionHelpers2D.StaticQuadInput { origin = this.origin, size = this.size };
                var line = new CollisionHelpers2D.LineInput { origin = rhs.origin, direction = rhs.direction };
                if (CollisionHelpers2D.DCD.GetQuadLineOverlap(sq, line, out var overlap))
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
                // TODO(alucbert): implement CCD for quad-ray intersection
                return false;
            }
            throw new System.Exception($"Unsupported collision type between {typeof(CircleCollider)} and {collider.GetType()}");
        }
    }
}
