using UnityEngine;

public class SpringTailScaler : MonoBehaviour
{
    public GameObject LeftEnd;
    public GameObject RightEnd;
    private Renderer _renderer;

    public void Start()
    {
        Debug.Assert(this.LeftEnd != null);
        Debug.Assert(this.RightEnd != null);
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

    private float GetLeftEndPosition()
    {
        float extent = 0f;
        if (this.LeftEnd.TryGetComponent(out Renderer renderer))
            extent = renderer.bounds.extents.x;
        return this.LeftEnd.transform.position.x + extent;
    }

    private float GetRightEndPosition()
    {
        float extent = 0f;
        if (this.RightEnd.TryGetComponent(out Renderer renderer))
            extent = renderer.bounds.extents.x;
        return this.RightEnd.transform.position.x - extent;
    }

    public void Scale()
    {
        float leftEndPosition = this.GetLeftEndPosition();
        float rightEndPosition = this.GetRightEndPosition();
        float distance = rightEndPosition - leftEndPosition;
        this.transform.position = new Vector3(
            leftEndPosition + (distance / 2f),
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
