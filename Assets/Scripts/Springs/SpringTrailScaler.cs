using UnityEngine;

[RequireComponent(typeof(GDS.Physics.SpringJoint2D))]
public class SpringTrailScaler : MonoBehaviour
{
    private Renderer _renderer;
    private GDS.Physics.SpringJoint2D _joint;

    public void Start()
    {
        this._joint = this.GetComponent<GDS.Physics.SpringJoint2D>();
        Debug.Assert(this._joint != null);
        this._renderer = this.GetComponent<Renderer>();
        Debug.Assert(this._renderer != null);
        Vector2 startScale = this.transform.localScale;
        this.Scale();
        Vector2 startTiling = this._renderer.material.mainTextureScale;
        this._renderer.material.mainTextureScale = new Vector2(
            this.transform.localScale.x * startTiling.x / startScale.x,
            this.transform.localScale.y * startTiling.y / startScale.y
        );
    }

    public void Scale()
    {
        float distance = this._joint.RightEnd.min.x - this._joint.LeftEnd.max.x;
        this.transform.position = new Vector3(
            this._joint.LeftEnd.max.x + (distance / 2f),
            this.transform.position.y,
            this.transform.position.z
        );
        this.transform.localScale = new Vector3(
            distance,
            this.transform.localScale.y,
            this.transform.localScale.z
        );
    }

    public void FixedUpdate()
    {
        this.Scale();
    }
}
