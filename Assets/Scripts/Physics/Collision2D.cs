using UnityEngine;

namespace GDS.Physics
{
    public class ContactPoint2D
    {
        public ContactPoint2D(GDS.Maths.Vector2 position, GDS.Maths.Vector2 normal)
        {
            this.position = position;
            this.normal = normal;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere((UnityEngine.Vector2)this.position, 0.01f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine((UnityEngine.Vector2)this.position, (UnityEngine.Vector2)(this.position + this.normal));
        }

        public GDS.Maths.Vector2 position;
        public GDS.Maths.Vector2 normal;
    }

    public struct Collision2D
    {
        public static bool Compute(ACollider2D from, ACollider2D to, out Collision2D collision)
        {
            collision = new Collision2D
            {
                from = from,
                to = to,
                contacts = null,
                toi = 0f
            };
            return from.GetContactPoints(to, out collision.contacts, out collision.toi);
        }

        public void DrawGizmos()
        {
            for (int i = 0; i < this.contacts.Length; ++i)
                this.contacts[i].DrawGizmos();
        }

        public ACollider2D from;
        public ACollider2D to;
        public ContactPoint2D[] contacts;
        /// <summary>
        /// Roughly estimated time of impact (not considering forces).
        /// </summary>
        public float toi;
    }
}
