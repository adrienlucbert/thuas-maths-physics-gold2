using UnityEngine;

namespace GDS.Physics
{
    /// <summary>
    /// This script is responsible for performing a rudimentory collision
    /// check on the children of the game object it's attached to. This way,
    /// I can isolate collision checks per simulation.
    /// The collision check is very naive, it simply checks every possible
    /// colliders pairs, without performing any broad-phase culling or anything.
    /// </summary>
    public class CollisionsManager : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            ACollider2D[] colliders = this.GetComponentsInChildren<ACollider2D>();
            for (int i = 0; i < colliders.Length; ++i)
            {
                for (int j = 0; j < colliders.Length; ++j)
                {
                    // Avoid checking collision of an object with itself
                    if (i == j)
                        continue;
                    // Static objects don't get affected by collision responses,
                    // we can avoid computing the collision response for them
                    if (colliders[i].isStatic)
                        continue;
                    // If a collision happened, resolve it
                    if (Collision2D.Compute(colliders[i], colliders[j], out Collision2D collision))
                        colliders[i].GetComponent<ACollisionResolver2D>().Resolve(collision);
                }
            }
        }
    }
}