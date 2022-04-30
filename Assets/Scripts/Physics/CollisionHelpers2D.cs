using UnityEngine;

namespace GDS.Physics
{
    public static class CollisionHelpers2D
    {
        public class CollisionData
        {
            public Maths.Vector2 position;
            public Maths.Vector2 normal;
        }

        public class LineInput
        {
            public Maths.Vector2 origin;
            public Maths.Vector2 direction;
        }

        public class StaticCircleInput
        {
            public Maths.Vector2 center;
            public float radius;
        }

        public class MovingCircleInput : StaticCircleInput
        {
            public Maths.Vector2 speed;
        }

        /// <summary>
        /// Continuous collision detection
        /// </summary>
        public static class CCD
        {
            public class CCDCollisionData : CollisionData
            {
                public float toi;
            }

            public static bool GetCircleLineIntersection(MovingCircleInput circle, LineInput line, out CCDCollisionData collision, float maxTravelTime = 0f)
            {
                collision = null;
                float maxTravelAmount = 0f; // only used if maxTravelTime != 0f
                // Get possible intersection between circle direction and line
                if (!Maths.Vector2.Intersect(circle.center, circle.speed, line.origin, line.direction, out Maths.Vector2 centerLineIntersection))
                    return false;
                collision = new CCDCollisionData { };
                // Get closest point to the circle on the line
                GDS.Maths.Vector2 closestPointOnLine = line.origin + (circle.center - line.origin).Project(line.direction);
                GDS.Maths.Vector2 vectorToClosestPointOnLine = closestPointOnLine - circle.center;

                // Make sure that the circle is moving towards the closest point
                // on the line
                if (circle.speed.normalized.Dot(vectorToClosestPointOnLine) <= 0f)
                    return false;

                float velocity = circle.speed.magnitude;
                float sqrDistanceToLine = vectorToClosestPointOnLine.sqrMagnitude;

                if (maxTravelTime != 0f)
                {
                    maxTravelAmount = velocity * maxTravelTime;
                    // No collision possible if movement amount is less than the
                    // distance to the closest point on line
                    if (maxTravelAmount * maxTravelAmount < sqrDistanceToLine)
                        return false;
                }

                // Find and normalize normal vector
                float distanceToLine = Mathf.Sqrt(sqrDistanceToLine);
                collision.normal = vectorToClosestPointOnLine * -1f / distanceToLine;

                // Get the circle position at time of collision (considering radius)
                collision.position = centerLineIntersection - (circle.radius * ((circle.center - centerLineIntersection).magnitude / distanceToLine) * (circle.speed / circle.speed.magnitude));
                float distanceToi = circle.center.Distance(collision.position);

                if (maxTravelTime != 0f)
                {
                    // Make sure that the distance the circle has to move to
                    // touch the line is not greated than the movement amount
                    if (maxTravelAmount < distanceToi)
                        return false;
                }

                collision.toi = distanceToi / velocity;
                return true;
            }

            public static bool GetCircleCircleIntersection(MovingCircleInput c1, StaticCircleInput c2, out CCDCollisionData collision, float maxTravelTime = 0f)
            {
                collision = null;
                Maths.Vector2 vectorBetweenCenters = c2.center - c1.center;
                float distanceBetweenCenters = vectorBetweenCenters.magnitude;
                float radiiSum = c1.radius + c2.radius;
                float velocity = c1.speed.magnitude;
                Maths.Vector2 n = c1.speed / velocity; // normalize speed
                float maxTravelAmount = 0f; // only used if maxTravelTime != 0f

                if (maxTravelTime != 0f)
                {
                    maxTravelAmount = velocity * maxTravelTime;
                    float minDistanceBetweenCircles = distanceBetweenCenters - radiiSum;
                    // No collision possible if movement amount is less than the
                    // minimum distance between the 2 circles
                    if (maxTravelAmount < minDistanceBetweenCircles)
                        return false;
                }

                // Make sure that c1 is moving towards c2
                float d = n.Dot(vectorBetweenCenters);
                if (d <= 0f)
                    return false;

                float f = (distanceBetweenCenters * distanceBetweenCenters) - (d * d);
                // Make sure the closest that c1 will get to c2 is less than the
                // sum of their radii
                float sqrRadiiSum = radiiSum * radiiSum;
                if (f >= sqrRadiiSum)
                    return false;

                // Find the closest distance to impact
                float t = sqrRadiiSum - f;
                if (t < 0f)
                    return false;
                float distanceToi = d - Mathf.Sqrt(t);

                if (maxTravelTime != 0f)
                {
                    // Make sure that the distance c1 has to move to touch c2 is
                    // not greated than the movement amount
                    if (maxTravelAmount < distanceToi)
                        return false;
                }

                float toi = distanceToi / velocity;
                Maths.Vector2 posAtToi = c1.center + (c1.speed * toi);
                Maths.Vector2 normal = (c2.center - posAtToi).normalized;
                collision = new CCDCollisionData
                {
                    position = posAtToi,
                    normal = normal,
                    toi = toi
                };
                return true;
            }

            public static bool GetCircleCircleIntersection(MovingCircleInput c1, MovingCircleInput c2, out CCDCollisionData collision, float maxTravelTime = 0f)
            {
                collision = null;
                Maths.Vector2 relativeSpeed = c1.speed - c2.speed;
                if (!GetCircleCircleIntersection(new MovingCircleInput { center = c1.center, radius = c1.radius, speed = relativeSpeed }, (StaticCircleInput)c2, out var relativeCollision, maxTravelTime))
                    return false;
                Maths.Vector2 c1AtToi = c1.center + (c1.speed * relativeCollision.toi);
                Maths.Vector2 c2AtToi = c2.center + (c2.speed * relativeCollision.toi);
                Maths.Vector2 normal = (c2AtToi - c1AtToi).normalized;
                collision = new CCDCollisionData
                {
                    position = c1AtToi,
                    normal = normal,
                    toi = relativeCollision.toi
                };
                return true;
            }
        }

        /// <summary>
        /// Discrete collision detection
        /// </summary>
        public static class DCD
        {
            public class DCDCollisionData : CollisionData
            {
                public float penetration;
            }

            public static bool GetCircleLineOverlap(StaticCircleInput circle, LineInput line, out DCDCollisionData collision)
            {
                GDS.Maths.Vector2 projection = line.origin + (circle.center - line.origin).Project(line.direction);
                GDS.Maths.Vector2 normal = circle.center - projection;
                float sqrDistance = normal.sqrMagnitude;
                float sqrRadius = circle.radius * circle.radius;
                if (sqrDistance > sqrRadius)
                {
                    collision = null;
                    return false;
                }
                normal = normal.normalized;
                float distance = Mathf.Sqrt(sqrDistance);
                float penetration = circle.radius - distance;
                GDS.Maths.Vector2 contact = projection - (normal * penetration);
                penetration = contact.Distance(projection);
                collision = new DCDCollisionData
                {
                    position = contact,
                    normal = normal,
                    penetration = penetration
                };
                return true;
            }

            public static bool GetCircleCircleOverlap(StaticCircleInput c1, StaticCircleInput c2, out DCDCollisionData collision)
            {
                float sqrDistance = c1.center.SqrDistance(c2.center);
                float sqrRadiiSum = (c1.radius + c2.radius) * (c1.radius + c2.radius);
                if (sqrDistance > sqrRadiiSum)
                {
                    collision = null;
                    return false;
                }
                float distance = Mathf.Sqrt(sqrDistance);
                float penetration = c1.radius - (distance - c2.radius);
                // normalize normal vector between circles centers
                GDS.Maths.Vector2 normal = (c1.center - c2.center) / distance;
                GDS.Maths.Vector2 contact = c1.center - (normal * c1.radius);
                collision = new DCDCollisionData
                {
                    position = contact,
                    normal = normal,
                    penetration = penetration
                };
                return true;
            }
        }
    }
}
