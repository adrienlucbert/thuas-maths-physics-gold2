using UnityEngine;
using GDS.Physics;

public class SimulationManager : MonoBehaviour
{
    private CollisionsManager __collisionsManager;
    private CollisionsManager _collisionsManager
    {
        get
        {
            if (this.__collisionsManager == null)
                this.__collisionsManager = this.GetComponent<CollisionsManager>();
            return this.__collisionsManager;
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (this._collisionsManager != null)
    //     {
    //         this._collisionsManager.CheckCollisions();
    //         this._collisionsManager.DrawGizmos();
    //         this._collisionsManager.ClearCollisions();
    //     }
    // }

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
