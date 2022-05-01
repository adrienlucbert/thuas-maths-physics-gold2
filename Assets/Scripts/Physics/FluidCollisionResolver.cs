using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class FluidCollisionResolver : ACollisionResolver2D
    {
        public override void Resolve(GDS.Physics.Collision2D collision)
        {
            // The following assumes that the fluid is defined by a quad collider
            QuadCollider lhs = collision.from as QuadCollider;
            if (lhs == null)
                throw new System.Exception($"Unsupported fluid collider type {collision.from.GetType()}");

            // Get the area of the collider that is immersed in the fluid
            float immersedArea = 0f;
            if (collision.to.GetType() == typeof(QuadCollider))
            {
                QuadCollider rhs = collision.to as QuadCollider;
                float surfaceHeight = lhs.origin.y + lhs.size.y;
                float colliderBottomToSurface = surfaceHeight - rhs.origin.y;
                immersedArea = rhs.GetArea() * Mathf.Min(rhs.size.y, colliderBottomToSurface);
            }
            else
            {
                throw new System.Exception($"Unsupported collider type {collision.to.GetType()}");
            }

            // Calculate the buoyant force applied
            float fluidMass = lhs.density * immersedArea;
            Maths.Vector3 force = -fluidMass * new Maths.Vector3(0f, -9.81f, 0f);
            collision.to.Forces?.AddOneTimeForce(new Force(force, Force.Type.Force, "buoyancy"));
        }
    }
}
