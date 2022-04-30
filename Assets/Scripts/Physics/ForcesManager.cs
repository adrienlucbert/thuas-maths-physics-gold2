using System.Collections.Generic;
using UnityEngine;

namespace GDS.Physics
{
    [RequireComponent(typeof(ACollider2D))]
    public class ForcesManager : MonoBehaviour
    {
        public Maths.Vector3 Speed = Maths.Vector3.zero;
        public float SpeedAmount = 0f;

        [SerializeField] private List<Force> _forces;
        private ACollider2D _collider;

        private void Start()
        {
            this._collider = this.GetComponent<ACollider2D>();
        }

        public void Apply()
        {
            Maths.Vector3 forcesSum = Maths.Vector3.zero;
            float mass = this._collider.GetMass();
            foreach (Force force in this._forces)
                forcesSum += force.Compute(mass, Time.deltaTime);
            // Discard velocity change forces (single-use)
            this._forces.RemoveAll(force => force.type == Force.Type.VelocityChange);
            this.Speed += forcesSum;
            this.SpeedAmount = this.Speed.magnitude;
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
