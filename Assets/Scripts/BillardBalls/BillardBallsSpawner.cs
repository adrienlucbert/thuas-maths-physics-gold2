using UnityEngine;

[ExecuteInEditMode]
public class BillardBallsSpawner : MonoBehaviour
{
    [Range(1, 20)]
    public int BallsCount = 10;
    public GameObject BallPrefab;
    public GameObject Container;
    public int RandomSeed = 0;
    public Vector2 xBounds;
    public Vector2 yBounds;
    public Vector2 speedBounds;
    private bool _needsRespawn = false;

    private void OnValidate()
    {
#if UNITY_EDITOR
        this._needsRespawn = true;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (this._needsRespawn)
        {
            this.Clear();
            this.Spawn();
        }
#endif
    }

    private void Clear()
    {
        for (var i = this.Container.transform.childCount; i > 0; --i)
        {
            DestroyImmediate(this.Container.transform.GetChild(0).gameObject);
        }
    }

    private void Spawn()
    {
        var rng = new System.Random(this.RandomSeed);
        float ballRadius = this.BallPrefab.GetComponent<GDS.Physics.CircleCollider>().radius;
        for (int i = 0; i < this.BallsCount; ++i)
        {
            float x = rng.Next((int)((this.xBounds[0] + ballRadius) * 1000), (int)((this.xBounds[1] - ballRadius) * 1000)) / 1000f;
            float y = rng.Next((int)((this.yBounds[0] + ballRadius) * 1000), (int)((this.yBounds[1] - ballRadius) * 1000)) / 1000f;
            GameObject ball = Instantiate(this.BallPrefab, this.Container.transform, false);
            ball.transform.Translate(new Vector3(x, y, 0f));
            GDS.Maths.Vector3 startVelocity = new GDS.Maths.Vector3(
                rng.Next((int)(this.speedBounds[0] * 1000), (int)(this.speedBounds[1] * 1000)) / 1000f,
                rng.Next((int)(this.speedBounds[0] * 1000), (int)(this.speedBounds[1] * 1000)) / 1000f,
                0f
            );
            ball.GetComponent<GDS.Physics.ForcesManager>().AddOneTimeForce(new GDS.Physics.Force(startVelocity, GDS.Physics.Force.Type.VelocityChange));
        }
    }
}
