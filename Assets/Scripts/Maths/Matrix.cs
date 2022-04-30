namespace GDS.Maths
{
    namespace detail
    {
        /// <summary>
        /// Generic floats matrix class.
        /// </summary>
        public class Matrix
        {
            public int rows
            {
                get;
                private set;
            }
            public int columns
            {
                get;
                private set;
            }
            private float[,] values;
            public bool isSquare => this.rows == this.columns;

            public float determinent
            {
                get
                {
                    UnityEngine.Debug.Assert(this.isSquare,
                        "Unable to calculate the determinent of a non-square matrix.");
                    int n = this.rows;
                    if (n == 2)
                    {
                        // Avoid expensive calculations for smaller matrices
                        return (this[0, 0] * this[1, 1]) - (this[1, 0] * this[0, 1]);
                    }
                    // Determinant is approximated by reducing the matrix to an upper triangle matrix.
                    // The determinant is then the product of the main diagonal entries
                    // cf: https://integratedmlai.com/find-the-determinant-of-a-matrix-with-pure-python-without-numpy-or-scipy/
                    Matrix triangle = this.Copy();

                    for (int diag = 0; diag < n; ++diag)
                    {
                        for (int row = diag + 1; row < n; ++row)
                        {
                            if (triangle[diag, diag] == 0f)
                            {
                                triangle[diag, diag] = 0.0000000000000000001f;
                            }
                            float crScaler = triangle[row, diag] / triangle[diag, diag];
                            for (int col = 0; col < n; ++col)
                            {
                                triangle[row, col] -= crScaler * triangle[diag, col];
                            }
                        }
                    }
                    float product = 1f;
                    for (int diag = 0; diag < n; ++diag)
                    {
                        product *= triangle[diag, diag];
                    }
                    return product;
                }
            }

            /// <summary>
            /// Creates a matrix of given rows and columns.
            /// </summary>
            /// <param name="rows">Number of rows</param>
            /// <param name="columns">Number of columns</param>
            public Matrix(int rows, int columns)
            {
                this.rows = rows;
                this.columns = columns;
                this.values = new float[rows, columns];
            }

            public static Matrix FromRows(params float[][] rows)
            {
                Matrix m = new Matrix(rows.Length, rows[0].Length);
                for (int i = 0; i < m.rows; ++i)
                {
                    UnityEngine.Debug.Assert(rows[i].Length == m.columns,
                        "All rows must have the same number of columns");
                    for (int j = 0; j < m.columns; ++j)
                        m.values[i, j] = rows[i][j];
                }
                return m;
            }

            public static Matrix FromColumns(params float[][] columns)
            {
                Matrix m = new Matrix(columns[0].Length, columns.Length);
                for (int i = 0; i < m.rows; ++i)
                {
                    UnityEngine.Debug.Assert(columns[i].Length == m.rows,
                        "All columns must have the same number of rows");
                    for (int j = 0; j < m.columns; ++j)
                        m.values[i, j] = columns[j][i];
                }
                return m;
            }

            public Matrix Copy()
            {
                Matrix m = new Matrix(this.rows, this.columns);
                for (int i = 0; i < this.rows; ++i)
                    for (int j = 0; j < this.columns; ++j)
                        m[i, j] = this[i, j];
                return m;
            }

            /// <summary>
            /// Create a matrix of type T from this matrix.
            /// </summary>
            public T As<T>() where T : Matrix, new()
            {
                var m = new T();
                UnityEngine.Debug.Assert(m.rows == this.rows && m.columns == this.columns,
                    $"Invalid cast to {typeof(T).Name}: sizes differ");
                for (int i = 0; i < this.rows; ++i)
                    for (int j = 0; j < this.columns; ++j)
                        m[i, j] = this[i, j];
                return m;
            }

            /// <summary>
            /// Access element in matrix at the given row and columns.
            /// </summary>
            /// <param name="row">Row index</param>
            /// <param name="column">Column index</param>
            public float this[int row, int column]
            {
                get { return this.values[row, column]; }
                set { this.values[row, column] = value; }
            }

            public static Matrix zero(int rows, int columns)
            {
                Matrix m = new Matrix(rows, columns);
                for (int i = 0; i < rows; ++i)
                    for (int j = 0; j < columns; ++j)
                        m[i, j] = 0f;
                return m;
            }

            public static Matrix one(int rows, int columns)
            {
                Matrix m = new Matrix(rows, columns);
                for (int i = 0; i < rows; ++i)
                    for (int j = 0; j < columns; ++j)
                        m[i, j] = 1f;
                return m;
            }

            public static Matrix identity(int size)
            {
                Matrix m = new Matrix(size, size);
                for (int i = 0; i < size; ++i)
                    for (int j = 0; j < size; ++j)
                        m[i, j] = (i == j ? 1f : 0f);
                return m;
            }

            public static Matrix operator +(Matrix lhs, Matrix rhs)
            {
                UnityEngine.Debug.Assert(lhs.rows == rhs.rows && lhs.columns == rhs.columns,
                    "Matrices must have the same number of rows and columns to be added.");
                Matrix m = new Matrix(lhs.rows, lhs.columns);
                for (int i = 0; i < lhs.rows; ++i)
                    for (int j = 0; j < lhs.columns; ++j)
                        m[i, j] = lhs[i, j] + rhs[i, j];
                return m;
            }

            public static Matrix operator -(Matrix lhs, Matrix rhs)
            {
                UnityEngine.Debug.Assert(lhs.rows == rhs.rows && lhs.columns == rhs.columns,
                    "Matrices must have the same number of rows and columns to be substracted.");
                Matrix m = new Matrix(lhs.rows, lhs.columns);
                for (int i = 0; i < lhs.rows; ++i)
                    for (int j = 0; j < lhs.columns; ++j)
                        m[i, j] = lhs[i, j] - rhs[i, j];
                return m;
            }

            public static Matrix operator *(Matrix lhs, Matrix rhs)
            {
                UnityEngine.Debug.Assert(lhs.columns == rhs.rows,
                    "The number of columns in the first matrix must be equal to the number of rows in the second matrix to multiply them.");
                Matrix m = Matrix.zero(lhs.rows, rhs.columns);
                for (int i = 0; i < m.rows; ++i)
                    for (int j = 0; j < m.columns; ++j)
                        for (int k = 0; k < lhs.columns; ++k)
                            m[i, j] += lhs[i, k] * rhs[k, j];
                return m;
            }

            public static bool operator ==(Matrix lhs, Matrix rhs)
            {
                if (lhs.rows != rhs.rows || lhs.columns != rhs.columns)
                    return false;
                for (int i = 0; i < lhs.rows; ++i)
                    for (int j = 0; j < lhs.columns; ++j)
                        if (lhs[i, j] != rhs[i, j])
                            return false;
                return true;
            }

            public static bool operator !=(Matrix lhs, Matrix rhs)
            {
                return !(lhs == rhs);
            }

            public override bool Equals(object rhs)
            {
                if (rhs == null || !(rhs is Matrix))
                    return false;
                return this == (rhs as Matrix);
            }

            public override int GetHashCode()
            {
                return (this.values as System.Collections.IStructuralEquatable)
                  .GetHashCode(System.Collections.Generic.EqualityComparer<float>.Default);
            }

            /// <summary>
            /// Create a string representation of this matrix.
            /// </summary>
            public override string ToString()
            {
                string s = "";
                for (int i = 0; i < this.rows; ++i)
                {
                    for (int j = 0; j < this.columns; ++j)
                    {
                        s += this[i, j].ToString("0.00");
                        if (j != this.columns - 1)
                            s += '\t';
                    }
                    if (i != this.rows - 1)
                        s += '\n';
                }
                return s;
            }

            public Matrix transpose
            {
                get
                {
                    Matrix m = new Matrix(this.rows, this.columns);
                    for (int i = 0; i < this.rows; ++i)
                        for (int j = 0; j < this.columns; ++j)
                            m[j, i] = this[i, j];
                    return m;
                }
            }
        }

        /// <summary>
        /// Matrix size definition.
        /// </summary>
        public interface IMatrixSize
        {
            public int rows { get; }
            public int columns { get; }
        }

        /// <summary>
        /// Generic matrix base class, which casts Matrix method's results to
        /// the given child matrix class.
        /// </summary>
        public class MatrixBase<T, M> : Matrix
          where T : IMatrixSize, new()
          where M : MatrixBase<T, M>, new()
        {
            private static T _size = new T();

            public MatrixBase()
              : base(MatrixBase<T, M>._size.rows, MatrixBase<T, M>._size.columns)
            {
            }

            /// <summary>
            /// Create a matrix from a list of rows.
            /// </summary>
            /// <param name="rows">Variable number of rows to create the matrix from</param>
            public static new M FromRows(params float[][] rows)
            {
                UnityEngine.Debug.Assert(rows.Length == MatrixBase<T, M>._size.rows && rows[0].Length == MatrixBase<T, M>._size.columns,
                    $"Expected {MatrixBase<T, M>._size.rows} rows and {MatrixBase<T, M>._size.columns} columns, got {rows.Length} rows and {rows[0].Length} columns.");
                return Matrix.FromRows(rows).As<M>();
            }

            /// <summary>
            /// Create a matrix from a list of columns.
            /// </summary>
            /// <param name="rows">Variable number of columns to create the matrix from</param>
            public static new M FromColumns(params float[][] columns)
            {
                UnityEngine.Debug.Assert(columns[0].Length == MatrixBase<T, M>._size.rows && columns.Length == MatrixBase<T, M>._size.columns,
                    $"Expected {MatrixBase<T, M>._size.rows} rows and {MatrixBase<T, M>._size.columns} columns, got {columns[0].Length} rows and {columns.Length} columns.");
                return Matrix.FromColumns(columns).As<M>();
            }

            /// <summary>
            /// Create a matrix of zeros of the given size.
            /// </summary>
            /// <param name="rows">Number of rows</param>
            /// <param name="columns">Number of columns</param>
            public static new M zero
            {
                get { return detail.Matrix.zero(MatrixBase<T, M>._size.rows, MatrixBase<T, M>._size.columns).As<M>(); }
            }

            /// <summary>
            /// Create a matrix of ones of the given size.
            /// </summary>
            /// <param name="rows">Number of rows</param>
            /// <param name="columns">Number of columns</param>
            public static new M one
            {
                get { return detail.Matrix.one(MatrixBase<T, M>._size.rows, MatrixBase<T, M>._size.columns).As<M>(); }
            }

            /// <summary>
            /// Create an identity matrix of given size
            /// </summary>
            /// <param name="size">Matrix size</param>
            public static new M identity
            {
                get
                {
                    UnityEngine.Debug.Assert(MatrixBase<T, M>._size.rows == MatrixBase<T, M>._size.columns,
                        "An identity matrix can only be square");
                    return detail.Matrix.identity(MatrixBase<T, M>._size.rows).As<M>();
                }
            }

            /// <summary>
            /// Create a copy of this matrix.
            /// </summary>
            public new M Copy()
            {
                return base.Copy().As<M>();
            }

            public static M operator +(MatrixBase<T, M> lhs, MatrixBase<T, M> rhs)
            {
                return ((lhs as Matrix) + (rhs as Matrix)).As<M>();
            }

            public static M operator -(MatrixBase<T, M> lhs, MatrixBase<T, M> rhs)
            {
                return ((lhs as Matrix) - (rhs as Matrix)).As<M>();
            }

            public static Matrix operator *(MatrixBase<T, M> lhs, Matrix rhs)
            {
                return (lhs as Matrix) * rhs;
            }
            public static M operator *(MatrixBase<T, M> lhs, MatrixBase<T, M> rhs)
            {
                return ((lhs as Matrix) * (rhs as Matrix)).As<M>();
            }

            /// <summary>
            /// Create the transpose of this matrix.
            /// </summary>
            public new M transpose
            {
                get { return base.transpose.As<M>(); }
            }
        }
    }

    namespace MatrixSizes
    {
        public struct MatrixSize2x2 : detail.IMatrixSize
        {
            public int rows { get { return 2; } }
            public int columns { get { return 2; } }
        }

        public struct MatrixSize3x3 : detail.IMatrixSize
        {
            public int rows { get { return 3; } }
            public int columns { get { return 3; } }
        }

        public struct MatrixSize4x4 : detail.IMatrixSize
        {
            public int rows { get { return 4; } }
            public int columns { get { return 4; } }
        }
    }

    public class Matrix2x2 : detail.MatrixBase<MatrixSizes.MatrixSize2x2, Matrix2x2>
    {
    }

    public class Matrix3x3 : detail.MatrixBase<MatrixSizes.MatrixSize3x3, Matrix3x3>
    {
    }

    public class Matrix4x4 : detail.MatrixBase<MatrixSizes.MatrixSize4x4, Matrix4x4>
    {
        /// <summary>
        /// Creates a translation, rotation and scaling matrix.
        /// </summary>
        /// <param name="position">Translation vector</param>
        /// <param name="rotation">Rotation vector</param>
        /// <param name="scale">Scale vector</param>
        public static Matrix4x4 TRS(UnityEngine.Vector3 translation, UnityEngine.Vector3 rotation, UnityEngine.Vector3 scale)
        {
            var m = Matrix4x4.TranslationMatrix(translation);
            m = m * Matrix4x4.RotationMatrix(rotation);
            m = m * Matrix4x4.ScalingMatrix(scale);
            return m;
        }

        /// <summary>
        /// Create a UnityEngine.Matrix4x4 from this matrix.
        /// </summary>
        public UnityEngine.Matrix4x4 ToUnity()
        {
            var m = new UnityEngine.Matrix4x4();
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    m[i, j] = this[i, j];
            return m;
        }

        /// <summary>
        /// Create a Matrix4x4 from a UnityEngine.Matrix4x4.
        /// </summary>
        public static Matrix4x4 FromUnity(UnityEngine.Matrix4x4 matrix)
        {
            var m = new Matrix4x4();
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    m[i, j] = matrix[i, j];
            return m;
        }

        private static Matrix4x4 TranslationMatrix(UnityEngine.Vector3 translation)
        {
            return Matrix4x4.FromRows(
                new float[] { 1, 0, 0, translation.x },
                new float[] { 0, 1, 0, translation.y },
                new float[] { 0, 0, 1, translation.z },
                new float[] { 0, 0, 0, 1 }
            );
        }

        private static Matrix4x4 ScalingMatrix(UnityEngine.Vector3 scale)
        {
            return Matrix4x4.FromRows(
                new float[] { scale.x, 0, 0, 0 },
                new float[] { 0, scale.y, 0, 0 },
                new float[] { 0, 0, scale.z, 0 },
                new float[] { 0, 0, 0, 1 }
            );
        }

        private static Matrix4x4 RotationMatrix(UnityEngine.Vector3 rotation)
        {
            var m = Matrix4x4.RotateAroundAxis(UnityEngine.Vector3.up, rotation.y);
            m = m * Matrix4x4.RotateAroundAxis(UnityEngine.Vector3.right, rotation.x);
            m = m * Matrix4x4.RotateAroundAxis(UnityEngine.Vector3.forward, rotation.z);
            return m;
        }

        private static Matrix4x4 RotateAroundAxis(UnityEngine.Vector3 axis, float angle)
        {
            axis = (UnityEngine.Vector3)((GDS.Maths.Vector3)axis).normalized;
            angle = angle * UnityEngine.Mathf.PI / 180f; // convert degree angle to radians
            float c = UnityEngine.Mathf.Cos(angle);
            float s = UnityEngine.Mathf.Sin(angle);
            float t = 1 - UnityEngine.Mathf.Cos(angle);
            var m = Matrix4x4.FromRows(
                new float[] { t * axis.x * axis.x + c, t * axis.x * axis.y - s * axis.z, t * axis.x * axis.y + s * axis.y, 0 },
                new float[] { t * axis.x * axis.y + s * axis.z, t * axis.y * axis.y + c, t * axis.y * axis.z - s * axis.x, 0 },
                new float[] { t * axis.x * axis.z - s * axis.y, t * axis.y * axis.z + s * axis.x, t * axis.z * axis.z + c, 0 },
                new float[] { 0, 0, 0, 1 });
            return m;
        }
    }
}
