using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class ForcesManager : MonoBehaviour
    {
        private ACollider2D _collider;
        [SerializeField] private Force[] _forces;
        public Maths.Vector3 Speed { get; private set; } = Maths.Vector3.zero;

        private void Start()
        {
            this._collider = this.GetComponent<ACollider2D>();
        }

        private void Update()
        {
            /*Maths.Vector3 forcesSum = Maths.Vector3.zero;
            for (int i = 0; i < this._forces.Length; ++i)
                forcesSum = forcesSum + this._forces[i].Compute(this._collider.GetMass(), Time.deltaTime);
            this.Speed = this.Speed + forcesSum;
            this.transform.position = (Vector3)((Maths.Vector3)this.transform.position + this.Speed);*/
        }
    }
}
