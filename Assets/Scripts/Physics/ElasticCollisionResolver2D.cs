using UnityEngine;
// using UnityEditor;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class ElasticCollisionResolver2D : ACollisionResolver2D
    {
        public float Restitution = 1f; // Fully elastic by default

        public override void Resolve(GDS.Physics.Collision2D collision)
        {
            base.Resolve(collision);
            // GUIStyle style = new GUIStyle();
            // style.normal.textColor = Color.black;

            // Neglect current speed
            this.Forces.AddForce(new Force(this.Forces.Speed * -1f, Force.Type.VelocityChange));
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Add a force in the reflected direction
                GDS.Maths.Vector3 normal3D = new Maths.Vector3(contact.normal.x, contact.normal.y, 0f);
                if (collision.to.isStatic)
                {
                    Maths.Vector3 v1f = this.Forces.Speed.Reflect(normal3D) * this.Restitution;
                    this.Forces.AddForce(new Force(v1f, Force.Type.VelocityChange));
                }
                else
                {
                    Maths.Vector3 v1 = collision.from.Forces?.Speed ?? Maths.Vector3.zero;
                    Maths.Vector3 v2 = collision.to.Forces?.Speed ?? Maths.Vector3.zero;
                    float m1 = collision.from.GetMass();
                    float m2 = collision.to.GetMass();
                    float a1 = v1.Dot(normal3D);
                    float a2 = v2.Dot(normal3D);
                    float p = 2f * (a1 - a2) / (m1 + m2);

                    Maths.Vector3 v1f = (v1 - (p * m2 * normal3D)) * this.Restitution;
                    this.Forces.AddForce(new Force(v1f, Force.Type.VelocityChange));

                    Maths.Vector3 travelToToi = v1 * collision.toi;
                    Maths.Vector3 travelFromToi = v1f * (collision.step - collision.toi);
                    Maths.Vector3 posAtToi = (Maths.Vector3)this.transform.position + travelToToi;
                    Maths.Vector3 posAfterFrame = posAtToi + travelFromToi;

                    this.transform.position = (UnityEngine.Vector3)posAfterFrame;

                    // Gizmos.color = new Color(1f, 0.1f, 1f);
                    // Gizmos.DrawLine(this.transform.position, (UnityEngine.Vector3)posAtToi);
                    // Gizmos.DrawLine((UnityEngine.Vector3)posAtToi, (UnityEngine.Vector3)posAfterFrame);
                    // Gizmos.DrawSphere((UnityEngine.Vector3)posAfterFrame, 0.05f);
                    // Handles.Label((UnityEngine.Vector3)(posAtToi + Maths.Vector3.right * 0.2f), $"{v1}", style);
                }
            }
        }
    }
}
