using UnityEngine;

namespace GDS.Physics
{
    public abstract class ACollider2D : MonoBehaviour
    {
        public bool useCCD = true;
        public bool isStatic = false;
        public float density = 1f;
        [TagSelector] public string[] IgnoreTags = new string[] { };
        private ForcesManager _forces;
        public ForcesManager Forces
        {
            get
            {
                if (this._forces == null)
                    this._forces = this.GetComponent<ForcesManager>();
                return this._forces;
            }
        }

        public abstract float GetArea();
        public float GetMass()
        {
            return this.GetArea() * this.density;
        }
        public abstract bool GetContactPoints(ACollider2D collider, float step, out ContactPoint2D[] contacts, out float toi);
    }
}
