using UnityEngine;

namespace GDS.Physics
{
    [System.Serializable]
    public class ContactPoint2D
    {
        public ContactPoint2D(GDS.Maths.Vector2 position, GDS.Maths.Vector2 normal, float? penetration = null)
        {
            this.position = position;
            this.normal = normal;
            this.penetration = penetration;
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
        public float? penetration;
    }

    [System.Serializable]
    public struct Collision2D
    {
        public static bool Compute(ACollider2D from, ACollider2D to, float step, out Collision2D collision)
        {
            collision = new Collision2D
            {
                from = from,
                to = to,
                contacts = null,
                toi = 0f,
                step = step
            };
            return from.GetContactPoints(to, step, out collision.contacts, out collision.toi);
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
        /// Roughly estimated time of impact (not considering forces)
        /// </summary>
        public float toi;
        /// <summary>
        /// Time step to consider
        /// Usually Time.deltaTime, but can be changed for debug purposes
        /// </summary>
        public float step;
    }
}
