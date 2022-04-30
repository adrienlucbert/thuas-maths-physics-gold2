using UnityEngine;
using GDS.Physics;

public class SimulationManager : MonoBehaviour
{
    private CollisionsManager _collisionsManager;

    private void Awake()
    {
        this._collisionsManager = this.GetComponent<CollisionsManager>();
    }

    private void FixedUpdate()
    {
        if (this._collisionsManager != null)
        {
            this._collisionsManager.CheckCollisions();
            this._collisionsManager.ResolveCollisions();
        }
        this.ApplyForces();
    }

    private void ApplyForces()
    {
        foreach (ForcesManager forcesManager in this.GetComponentsInChildren<ForcesManager>())
            forcesManager.Apply();
    }
}
