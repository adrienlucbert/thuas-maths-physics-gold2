namespace GDS.Physics
{
    [System.Serializable]
    public class Force : Maths.Vector3
    {
        public enum Type
        {
            Force,
            Acceleration,
            Impulse,
            VelocityChange
        };

        public string name = null;
        public Type type = Type.Force;

        public Force(float x, float y, float z, Type type, string name = null)
          : base(x, y, z)
        {
            this.type = type;
            this.name = name;
        }
        public Force(Maths.Vector3 force, Type type, string name = null)
         : base(force.x, force.y, force.z)
        {
            this.type = type;
            this.name = name;
        }

        public Maths.Vector3 Compute(float mass, float timestep)
        {
            switch (this.type)
            {
                case Type.Force:
                    if (mass == 0.0f)
                        return Maths.Vector3.zero;
                    return this * (timestep / mass);

                case Type.Acceleration:
                    return this * timestep;

                case Type.Impulse:
                    if (mass == 0.0f)
                        return Maths.Vector3.zero;
                    return this / mass;

                case Type.VelocityChange:
                    return this;
            }
            throw new System.Exception($"Unsupported force type {this.type}");
        }
    }
}
