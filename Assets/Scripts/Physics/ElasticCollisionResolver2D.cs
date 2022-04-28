namespace GDS.Physics
{
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
                this.Forces.AddForce(new Force(this.Forces.Speed.Reflect(normal3D), Force.Type.VelocityChange));
            }
        }
    }
}
