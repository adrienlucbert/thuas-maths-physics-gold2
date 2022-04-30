using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ForcesManager))]
    public abstract class ACollisionResolver2D : MonoBehaviour
    {
        private ForcesManager _forces;
        protected ForcesManager Forces
        {
            get
            {
                if (this._forces == null)
                    this._forces = this.GetComponent<ForcesManager>();
                return this._forces;
            }
        }

        public abstract void Resolve(GDS.Physics.Collision2D collision);
    }
}
