namespace GDS.Physics
{
    public class InelasticCollisionResolver2D : ACollisionResolver2D
    {
        public float Damp = 0.9f;

        public override void Resolve(GDS.Physics.Collision2D collision)
        {
            base.Resolve(collision);
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Neglect current speed
                this.Forces.AddForce(new Force(this.Forces.Speed * -1f, Force.Type.VelocityChange));
                // Add a force in the reflected direction
                GDS.Maths.Vector3 normal3D = new GDS.Maths.Vector3(contact.normal.x, contact.normal.y, 0f);
                this.Forces.AddForce(new Force(this.Forces.Speed.Reflect(normal3D) * this.Damp, Force.Type.VelocityChange));
            }
        }
    }
}
