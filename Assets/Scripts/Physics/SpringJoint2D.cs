using UnityEngine;

namespace GDS.Physics
{
    public class SpringJoint2D : MonoBehaviour
    {
        public class SpringJointEnd
        {
            public GameObject gameObject;
            public Maths.Vector2 extents;
            public Maths.Vector2 stretch;
            public ForcesManager forces;
            private Renderer _renderer = null;

            public Maths.Vector2 min
              => new Maths.Vector2(
                   this.gameObject.transform.position.x - this.extents.x,
                   this.gameObject.transform.position.y - this.extents.y
                 );
            public Maths.Vector2 max
              => new Maths.Vector2(
                   this.gameObject.transform.position.x + this.extents.x,
                   this.gameObject.transform.position.y + this.extents.y
                 );

            public SpringJointEnd(GameObject gameObject)
            {
                this.gameObject = gameObject;
                gameObject.TryGetComponent(out this._renderer);
                gameObject.TryGetComponent(out this.forces);
                this.extents = new Maths.Vector2(
                    this._renderer?.bounds.extents.x ?? 0f,
                    this._renderer?.bounds.extents.y ?? 0f
                );
            }
        }

        [HideInInspector] public SpringJointEnd LeftEnd;
        [HideInInspector] public SpringJointEnd RightEnd;
        public Maths.Vector2 Stretch;
        [Tooltip("String constant k (N/m)")]
        public float Stiffness = 0f;

        [SerializeField] private GameObject _leftEnd;
        [SerializeField] private GameObject _rightEnd;
        private Maths.Vector2 _restLength;

        private void Start()
        {
            Debug.Assert(this._leftEnd != null);
            Debug.Assert(this._rightEnd != null);
            this.LeftEnd = new SpringJointEnd(this._leftEnd);
            this.RightEnd = new SpringJointEnd(this._rightEnd);
            this._restLength = new Maths.Vector2(
                (this.RightEnd.min.x - this.LeftEnd.max.x) * (1f / this.Stretch.x),
                (this.RightEnd.min.y - this.LeftEnd.max.y) * (1f / this.Stretch.y)
            );
        }

        private void FixedUpdate()
        {
            float leftBound = this.LeftEnd.gameObject.transform.position.x + this.LeftEnd.extents.x;
            float rightBound = this.RightEnd.gameObject.transform.position.x + this.RightEnd.extents.x;
            this.Stretch = (new Maths.Vector2(
                this.RightEnd.min.x - this.LeftEnd.max.x,
                this.RightEnd.min.y - this.LeftEnd.max.y
            ) / this._restLength);
            this.ApplyForces();
        }

        private void ApplyForces()
        {
            Maths.Vector2 displacement = Maths.Vector2.one - this.Stretch;
            Maths.Vector2 restorativeForce = displacement * (-1f) * this.Stiffness;
            this.ApplyForce(restorativeForce, this.LeftEnd);
            this.ApplyForce(restorativeForce, this.RightEnd);
        }

        private void ApplyForce(Maths.Vector2 force, SpringJointEnd end)
        {
            if (end.forces != null)
            {
                Maths.Vector2 forceDirection = new Maths.Vector2(
                    this.transform.position.x - end.gameObject.transform.position.x,
                    this.transform.position.y - end.gameObject.transform.position.y
                ).normalized;
                Maths.Vector2 finalForce = forceDirection * force;
                end.forces.AddOneTimeForce(new Force(new Maths.Vector3(finalForce.x, finalForce.y, 0f), Force.Type.Force));
            }
        }
    }
}
