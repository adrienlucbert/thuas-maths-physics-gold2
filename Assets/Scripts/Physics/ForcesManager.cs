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
        [SerializeField] private List<Force> _oneTimeForces;
        private ACollider2D _collider;

        private void Start()
        {
            this._collider = this.GetComponent<ACollider2D>();
        }

        public void Apply(float deltaTime)
        {
            Maths.Vector3 forcesSum = Maths.Vector3.zero;
            float mass = this._collider.GetMass();

            // Sum all forces
            foreach (Force force in this._forces)
                forcesSum += force.Compute(mass, deltaTime);
            foreach (Force force in this._oneTimeForces)
                forcesSum += force.Compute(mass, deltaTime);
            // Discard one-time forces after they've been used
            this._oneTimeForces.Clear();

            this.Speed += forcesSum;
            this.SpeedAmount = this.Speed.magnitude;
            this.transform.position = (Vector3)((Maths.Vector3)this.transform.position + this.Speed * deltaTime);
        }

        public void AddForce(Force force)
        {
            this._forces.Add(force);
        }

        public void AddOneTimeForce(Force force)
        {
            this._oneTimeForces.Add(force);
        }

        public bool UpdateForce(Force value)
        {
            Force force = this._forces.Find(force => force.name == value.name);
            if (force == null)
                return false;
            for (int i = 0; i < force.size; ++i)
                force[i] = value[i];
            return true;
        }

        public void UpdateOrAddForce(Force force)
        {
            if (!this.UpdateForce(force))
                this.AddForce(force);
        }

        public bool RemoveForce(Force force)
        {
            return this._forces.Remove(force);
        }

        public bool RemoveForce(string name)
        {
            return this._forces.RemoveAll(force => force.name == name) > 0;
        }
    }
}
