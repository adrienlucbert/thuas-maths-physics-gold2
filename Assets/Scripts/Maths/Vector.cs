using UnityEngine;

namespace GDS.Maths
{
    namespace detail
    {
        [System.Serializable]
        public class Vector
        {
            public int size => this.values.Length;
            [SerializeField] private float[] values;

            public Vector(int size)
            {
                this.values = new float[size];
            }

            public Vector(params float[] values)
            {
                this.values = values;
            }

            public void Set(params float[] values)
            {
                UnityEngine.Debug.Assert(this.size == values.Length,
                    $"Expected {this.size} elements, got {values.Length}");
                this.values = values;
            }

            public Vector Copy()
            {
                return new Vector((float[])this.values.Clone());
            }

            public T As<T>() where T : Vector, new()
            {
                var v = new T();
                UnityEngine.Debug.Assert(v.size == this.size,
                    $"Invalid cast to {typeof(T).Name}: sizes differ");
                for (int i = 0; i < this.size; ++i)
                    v[i] = this[i];
                return v;
            }

            public float this[int index]
            {
                get { return this.values[index]; }
                set { this.values[index] = value; }
            }

            public static Vector zero(int size)
            {
                var v = new Vector(size);
                for (int i = 0; i < size; ++i)
                    v[i] = 0f;
                return v;
            }

            public static Vector one(int size)
            {
                var v = new Vector(size);
                for (int i = 0; i < size; ++i)
                    v[i] = 1f;
                return v;
            }

            public float sqrMagnitude
            {
                get
                {
                    float sum = 0;
                    for (int i = 0; i < this.size; ++i)
                        sum += this[i] * this[i];
                    return sum;
                }
            }

            public float magnitude
            {
                get { return UnityEngine.Mathf.Sqrt(this.sqrMagnitude); }
            }

            public Vector normalized
            {
                get
                {
                    float norm = this.magnitude;
                    var v = this.Copy();
                    for (int i = 0; i < v.size; ++i)
                        v[i] = v[i] / norm;
                    return v;
                }
            }

            public float SqrDistance(Vector rhs)
            {
                return (this - rhs).sqrMagnitude;
            }

            public float Distance(Vector rhs)
            {
                return (this - rhs).magnitude;
            }

            /// <summary>
            /// Project `this` onto the given vector.
            /// Equivalent to https://docs.unity3d.com/ScriptReference/Vector3.Project.html
            /// </summary>
            /// <param name="onNormal">Vector onto which to project `this`</param>
            /// <returns>Projected vector</returns>
            public Vector Project(Vector onNormal)
            {
                Vector unit = onNormal.normalized;
                return unit * this.Dot(unit);
            }

            public float Dot(Vector rhs)
            {
                UnityEngine.Debug.Assert(this.size == rhs.size,
                    "Vectors must be the same size to perform a dot product.");
                float product = 0f;
                for (int i = 0; i < this.size; ++i)
                    product += this[i] * rhs[i];
                return product;
            }

            /// <summary>
            /// Reflects `this` off the plane defined by a normal.
            /// Equivalent to https://docs.unity3d.com/ScriptReference/Vector3.Reflect.html
            /// </summary>
            /// <param name="inNormal">Plane's normal vector (must be normalized)</param>
            /// <returns>Reflected vector</returns>
            public Vector Reflect(Vector inNormal)
            {
                return this - (this * inNormal) * 2f * inNormal;
            }

            public static Vector operator +(Vector lhs, Vector rhs)
            {
                UnityEngine.Debug.Assert(lhs.size == rhs.size,
                    "Vectors must be the same size to be added.");
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] += rhs[i];
                return v;
            }
            public static Vector operator +(Vector lhs, float rhs)
            {
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] += rhs;
                return v;
            }

            public static Vector operator -(Vector lhs, Vector rhs)
            {
                UnityEngine.Debug.Assert(lhs.size == rhs.size,
                    "Vectors must be the same size to be substracted.");
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] -= rhs[i];
                return v;
            }
            public static Vector operator -(Vector lhs, float rhs)
            {
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] -= rhs;
                return v;
            }

            public static Vector operator *(Vector lhs, Vector rhs)
            {
                UnityEngine.Debug.Assert(lhs.size == rhs.size,
                    "Vectors must be the same size to be multiplied.");
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] *= rhs[i];
                return v;
            }
            public static Vector operator *(Vector lhs, float rhs)
            {
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] *= rhs;
                return v;
            }

            public static Vector operator /(Vector lhs, Vector rhs)
            {
                UnityEngine.Debug.Assert(lhs.size == rhs.size,
                    "Vectors must be the same size to be divided.");
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] /= rhs[i];
                return v;
            }
            public static Vector operator /(Vector lhs, float rhs)
            {
                Vector v = lhs.Copy();
                for (int i = 0; i < lhs.size; ++i)
                    v[i] /= rhs;
                return v;
            }

            public static bool operator ==(Vector lhs, Vector rhs)
            {
                if (rhs is null)
                    return lhs is null;
                if (lhs.size != rhs.size)
                    return false;
                for (int i = 0; i < lhs.size; ++i)
                    if (lhs[i] != rhs[i])
                        return false;
                return true;
            }

            public static bool operator !=(Vector lhs, Vector rhs)
            {
                return !(lhs == rhs);
            }

            public override bool Equals(object rhs)
            {
                if (rhs is null || !(rhs is Vector))
                    return false;
                return this == (rhs as Vector);
            }

            public override int GetHashCode()
            {
                return (this.values as System.Collections.IStructuralEquatable)
                  .GetHashCode(System.Collections.Generic.EqualityComparer<float>.Default);
            }

            public override string ToString()
            {
                string s = "";
                for (int i = 0; i < this.size; ++i)
                {
                    s += this[i].ToString("0.00");
                    if (i != this.size - 1)
                        s += "\t";
                }
                return s;
            }
        }

        public interface IVectorSize
        {
            public int size { get; }
        }

        [System.Serializable]
        public class VectorBase<T, M> : Vector
          where T : IVectorSize, new()
          where M : VectorBase<T, M>, new()
        {
            private static T _size = new T();

            public VectorBase()
              : base(VectorBase<T, M>._size.size)
            {
            }

            public VectorBase(params float[] values)
              : base(values)
            {
                UnityEngine.Debug.Assert(values.Length == VectorBase<T, M>._size.size,
                    $"Expected {VectorBase<T, M>._size.size} values, got {values.Length}");
            }

            public static new M zero
            {
                get { return detail.Vector.zero(VectorBase<T, M>._size.size).As<M>(); }
            }

            public static new M one
            {
                get { return detail.Vector.one(VectorBase<T, M>._size.size).As<M>(); }
            }

            public new M Copy()
            {
                return base.Copy().As<M>();
            }

            public new M normalized
            {
                get { return base.normalized.As<M>(); }
            }

            public M Project(M onNormal)
            {
                return base.Project(onNormal as Vector).As<M>();
            }

            public M Reflect(M inNormal)
            {
                return base.Reflect(inNormal as Vector).As<M>();
            }

            public static M operator +(VectorBase<T, M> lhs, Vector rhs)
            {
                return ((lhs as Vector) + rhs).As<M>();
            }
            public static M operator +(VectorBase<T, M> lhs, float rhs)
            {
                return ((lhs as Vector) + rhs).As<M>();
            }
            public static M operator +(float lhs, VectorBase<T, M> rhs)
            {
                return ((rhs as Vector) + lhs).As<M>();
            }

            public static M operator -(VectorBase<T, M> lhs, Vector rhs)
            {
                return ((lhs as Vector) - rhs).As<M>();
            }
            public static M operator -(VectorBase<T, M> lhs, float rhs)
            {
                return ((lhs as Vector) - rhs).As<M>();
            }

            public static M operator *(VectorBase<T, M> lhs, Vector rhs)
            {
                return ((lhs as Vector) * rhs).As<M>();
            }
            public static M operator *(VectorBase<T, M> lhs, float rhs)
            {
                return ((lhs as Vector) * rhs).As<M>();
            }
            public static M operator *(float lhs, VectorBase<T, M> rhs)
            {
                return ((rhs as Vector) * lhs).As<M>();
            }

            public static M operator /(VectorBase<T, M> lhs, Vector rhs)
            {
                return ((lhs as Vector) / rhs).As<M>();
            }
            public static M operator /(VectorBase<T, M> lhs, float rhs)
            {
                return ((lhs as Vector) / rhs).As<M>();
            }
        }
    }

    namespace VectorSizes
    {
        public struct VectorSize2 : detail.IVectorSize
        {
            public int size { get { return 2; } }
        }

        public struct VectorSize3 : detail.IVectorSize
        {
            public int size { get { return 3; } }
        }

        public struct VectorSize4 : detail.IVectorSize
        {
            public int size { get { return 4; } }
        }
    }

    [System.Serializable]
    public class Vector2 : detail.VectorBase<VectorSizes.VectorSize2, Vector2>
    {
        public Vector2()
          : base()
        { }

        public Vector2(float x, float y)
          : base(x, y)
        { }

        public float x { get { return this[0]; } }
        public float y { get { return this[1]; } }

        public static Vector2 left { get { return new Vector2(-1, 0); } }
        public static Vector2 right { get { return new Vector2(1, 0); } }
        public static Vector2 down { get { return new Vector2(0, -1); } }
        public static Vector2 up { get { return new Vector2(0, 1); } }

        public static bool Intersect(Vector2 lhsOrigin, Vector2 lhsDir, Vector2 rhsOrigin, Vector2 rhsDir, out Vector2 intersection)
        {
            Vector2 v1i = lhsOrigin;
            Vector2 v1f = lhsOrigin + lhsDir;
            Vector2 v2i = rhsOrigin;
            Vector2 v2f = rhsOrigin + rhsDir;
            float a1 = v1f.y - v1i.y;
            float a2 = v2f.y - v2i.y;
            float b1 = v1i.x - v1f.x;
            float b2 = v2i.x - v2f.x;
            float c1 = (a1 * v1i.x) + (b1 * v1i.y);
            float c2 = (a2 * v2i.x) + (b2 * v2i.y);
            float d = Maths.Matrix2x2.FromRows(
                new float[] { a1, b1 },
                new float[] { a2, b2 }
            ).determinent;
            if (d == 0f)
            {
                intersection = null;
                return false;
            }
            float x = ((c1 * b2) - (b1 * c2)) / d;
            float y = ((a1 * c2) - (c1 * a2)) / d;
            intersection = new Vector2(x, y);
            return true;
        }

        public static explicit operator UnityEngine.Vector2(Vector2 v)
        {
            return new UnityEngine.Vector2(v[0], v[1]);
        }

        public static explicit operator Vector2(UnityEngine.Vector2 v)
        {
            return new Vector2(v[0], v[1]);
        }
    }

    [System.Serializable]
    public class Vector3 : detail.VectorBase<VectorSizes.VectorSize3, Vector3>
    {
        public Vector3()
          : base()
        { }

        public Vector3(float x, float y, float z)
          : base(x, y, z)
        { }

        public float x { get { return this[0]; } }
        public float y { get { return this[1]; } }
        public float z { get { return this[2]; } }

        public float r { get { return this[0]; } }
        public float g { get { return this[1]; } }
        public float b { get { return this[2]; } }

        public static Vector3 left { get { return new Vector3(-1, 0, 0); } }
        public static Vector3 right { get { return new Vector3(1, 0, 0); } }
        public static Vector3 down { get { return new Vector3(0, -1, 0); } }
        public static Vector3 up { get { return new Vector3(0, 1, 0); } }
        public static Vector3 back { get { return new Vector3(0, 0, -1); } }
        public static Vector3 forward { get { return new Vector3(0, 0, 1); } }

        public Vector3 Cross(Vector3 rhs)
        {
            return new Vector3(
                this.y * rhs.z - rhs.z * this.y,
                this.z * rhs.x - this.x * rhs.z,
                this.x * rhs.y - this.y * rhs.x
            );
        }

        public static explicit operator UnityEngine.Vector3(Vector3 v)
        {
            return new UnityEngine.Vector3(v[0], v[1], v[2]);
        }

        public static explicit operator Vector3(UnityEngine.Vector3 v)
        {
            return new Vector3(v[0], v[1], v[2]);
        }
    }

    [System.Serializable]
    public class Vector4 : detail.VectorBase<VectorSizes.VectorSize4, Vector4>
    {
        public Vector4()
          : base()
        { }

        public Vector4(float x, float y, float z, float w)
          : base(x, y, z, w)
        { }

        public float x { get { return this[0]; } }
        public float y { get { return this[1]; } }
        public float z { get { return this[2]; } }
        public float w { get { return this[3]; } }

        public float r { get { return this[0]; } }
        public float g { get { return this[1]; } }
        public float b { get { return this[2]; } }
        public float a { get { return this[3]; } }

        public static explicit operator UnityEngine.Vector4(Vector4 v)
        {
            return new UnityEngine.Vector4(v[0], v[1], v[2], v[3]);
        }

        public static explicit operator Vector4(UnityEngine.Vector4 v)
        {
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
    }
}
