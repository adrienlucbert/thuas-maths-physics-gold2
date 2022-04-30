namespace GDS.Physics
{
    public class LineCollider : ACollider2D
    {
        public GDS.Maths.Vector2 origin
            => new GDS.Maths.Vector2(this.transform.position.x, this.transform.position.y);
        public GDS.Maths.Vector2 direction;

        private void OnDrawGizmosSelected()
        {
        }

        public override float GetArea()
        {
            // A line has no area. To make it have an arbitrary
            // default mass of 1, I fake an area of 1.
            return 1f;
        }

        public override bool GetContactPoints(ACollider2D collider, float step, out ContactPoint2D[] contacts, out float toi)
        {
            throw new System.Exception($"Unsupported collision type between {typeof(LineCollider)} and {collider.GetType()}");
        }
    }
}
