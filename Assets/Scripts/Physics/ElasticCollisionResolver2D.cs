using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class ElasticCollisionResolver2D : ACollisionResolver2D
    {
        public override void Resolve(GDS.Physics.Collision2D collision)
        {
            base.Resolve(collision);
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Neglect current speed
                this.Forces.AddForce(new Force(this.Forces.Speed * -1f, Force.Type.VelocityChange));
                // Add a force in the reflected direction
                GDS.Maths.Vector3 normal3D = new GDS.Maths.Vector3(contact.normal.x, contact.normal.y, 0f);
                if (collision.to.isStatic)
                {
                    this.Forces.AddForce(new Force(this.Forces.Speed.Reflect(normal3D), Force.Type.VelocityChange));
                }
                else
                {
                    float m1 = collision.from.GetMass();
                    float m2 = collision.to.GetMass();
                    float v1i = collision.from.GetComponent<ForcesManager>().Speed.magnitude;
                    float v2i = collision.to.GetComponent<ForcesManager>()?.Speed.magnitude ?? 0f;
                    float v1f = ((m1 - m2) / (m2 + m1)) * v1i + ((2f * m2) / (m2 + m1)) * v2i;
                    this.Forces.AddForce(new Force(this.Forces.Speed.Reflect(normal3D).normalized * v1f, Force.Type.VelocityChange));
                }
            }
        }
    }
}
