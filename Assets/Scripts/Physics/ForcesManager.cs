using System.Collections.Generic;
using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class ForcesManager : MonoBehaviour
    {
        public Maths.Vector3 Speed = Maths.Vector3.zero;
        private float t3;
        private float t4;

        [SerializeField] private List<Force> _forces;
        private ACollider2D _collider;

        private void Start()
        {
            this._collider = this.GetComponent<ACollider2D>();
        }

        private void FixedUpdate()
        {
            Maths.Vector3 forcesSum = Maths.Vector3.zero;
            foreach (Force force in this._forces)
                forcesSum += force.Compute(this._collider.GetMass(), Time.deltaTime);
            this._forces.RemoveAll(force => force.type == Force.Type.VelocityChange);
            this.Speed += forcesSum;
            this.transform.position = (Vector3)((Maths.Vector3)this.transform.position + this.Speed * Time.deltaTime);
        }

        public void AddForce(Force force)
        {
            this._forces.Add(force);
        }

        public void RemoveForce(Force force)
        {
            this._forces.Remove(force);
        }
    }
}
