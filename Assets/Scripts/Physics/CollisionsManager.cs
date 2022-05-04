using System.Collections.Generic;
using System.Linq;
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
        [SerializeField]
        private List<Collision2D> collisions = new List<Collision2D> { };

        public void DrawGizmos()
        {
            foreach (Collision2D collision in this.collisions)
                collision.DrawGizmos();
        }

        public void CheckCollisions()
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
                    // Ignore collision if collider has one of the ignored tags
                    if (colliders[i].IgnoreTags.Any(tag => colliders[j].CompareTag(tag)))
                        continue;
                    // If a collision happened, add it to the list
                    if (Collision2D.Compute(colliders[i], colliders[j], Time.fixedDeltaTime, out Collision2D collision))
                        this.collisions.Add(collision);
                }
            }
        }

        public void ResolveCollisions()
        {
            foreach (Collision2D collision in this.collisions)
                collision.from.GetComponent<ACollisionResolver2D>().Resolve(collision);
            this.ClearCollisions();
        }

        public void ClearCollisions()
        {
            this.collisions.Clear();
        }
    }
}
