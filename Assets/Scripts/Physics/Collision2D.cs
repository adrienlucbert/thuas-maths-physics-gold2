using UnityEngine;

namespace GDS.Physics
{
    public class ContactPoint2D
    {
        public ContactPoint2D(GDS.Maths.Vector2 position, GDS.Maths.Vector2 normal, float penetration)
        {
            this.position = position;
            this.normal = normal;
            this.penetration = penetration;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((UnityEngine.Vector2)this.position, 0.05f);
            Gizmos.DrawLine((UnityEngine.Vector2)this.position, (UnityEngine.Vector2)(this.position + this.normal * this.penetration));
        }

        GDS.Maths.Vector2 position;
        GDS.Maths.Vector2 normal;
        public float penetration;
    }

    public struct Collision2D
    {
        public static bool Compute(ACollider2D from, ACollider2D to, out Collision2D collision)
        {
            collision = new Collision2D
            {
                collider = to,
                contacts = from.GetContactPoints(to)
            };
            return collision.contacts.Length > 0;
        }

        public void DrawGizmos()
        {
            for (int i = 0; i < this.contacts.Length; ++i)
                this.contacts[i].DrawGizmos();
        }

        ACollider2D collider;
        ContactPoint2D[] contacts;
    }
}
