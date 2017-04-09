#region Header Comments
/*****************************************************************************/
/* DoubleMatrix.cs                                                           */
/* ---------------                                                           */
/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* A decorator class for MatrixBase<Double> that provides additional         */
/* functionality such as matrix inversion, mathematical operations etc.      */
/*                                                                           */
/* This code is released to the public domain.                               */
/*****************************************************************************/
#endregion Header Comments

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearAlgebra.Matrices 
{
    public class DoubleMatrix 
    {

        public class RowAccessor
            : IEnumerable<DoubleMatrix>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private DoubleMatrix _matrix;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>RowAccessor</summary>
            /// <param name="dataView"></param>
            public RowAccessor(DoubleMatrix matrix)
            {
                _matrix = matrix;
            }
            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            public DoubleMatrix this[Int32 rowIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(rowIndex);
                }
                set
                {
                    if (value.ColumnCount != _matrix.ColumnCount || value.RowCount != 1)
                        throw new DimensionMismatchException();

                    DoubleMatrix targetRow = this.AsEnumerable().ElementAt(rowIndex);
                    for (Int32 i = 0; i < value.ColumnCount; i++)
                        targetRow[i, 0] = value[i, 0];
                }
            }

            #endregion Public Accessors


            #region Public Methods
            /******************/
            /* PUBLIC METHODS */
            /******************/

            /// <summary>AsEnumerable</summary>
            /// <returns></returns>
            public IEnumerable<DoubleMatrix> AsEnumerable()
            {
                int j = 0;
                while (j < _matrix.RowCount)
                    yield return _matrix.SubMatrix(new Int32Range(0, _matrix.ColumnCount), new Int32Range(j, ++j));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<DoubleMatrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }


            /// <summary>IEnumerable.GetEnumerator</summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion Public Methods

        }

        public class ColumnAccessor
            : IEnumerable<DoubleMatrix>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private DoubleMatrix _matrix;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>ColumnAccessor</summary>
            /// <param name="dataView"></param>
            public ColumnAccessor(DoubleMatrix matrix)
            {
                _matrix = matrix;
            }

            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            /// <summary>this</summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public DoubleMatrix this[Int32 columnIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(columnIndex);
                }
                set
                {
                    if (value.RowCount != _matrix.RowCount || value.ColumnCount != 1)
                        throw new DimensionMismatchException();

                    DoubleMatrix targetColumn = this.AsEnumerable().ElementAt(columnIndex);
                    for (int j = 0; j < value.RowCount; j++)
                        targetColumn[0, j] = value[0, j];
                }
            }

            #endregion Public Accessors


            #region Public Methods
            /******************/
            /* PUBLIC METHODS */
            /******************/

            /// <summary>AsEnumerable</summary>
            /// <returns></returns>
            public IEnumerable<DoubleMatrix> AsEnumerable()
            {
                int i = 0;
                while (i < _matrix.ColumnCount)
                    yield return _matrix.SubMatrix(new Int32Range(i, ++i), new Int32Range(0, _matrix.RowCount));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<DoubleMatrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }


            /// <summary>IEnumerable.GetEnumerator</summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion Public Methods

        }


        #region Private Static Members
        /**************************/
        /* PRIVATE STATIC MEMBERS */
        /**************************/
        private static Random _random = new Random(Environment.TickCount);

        #endregion Private Static Members


        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private MatrixBase<Double> _matrix;

        #endregion Private Members


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>
        /// 
        /// </summary>
        public DoubleMatrix() : this(0) { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rank"></param>
        public DoubleMatrix(int rank) : this(rank, rank) { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public DoubleMatrix(int columns, int rows) 
        {
            _matrix = new MatrixBase<Double>(columns, rows);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleArray"></param>
        public DoubleMatrix(Double[,] doubleArray) 
        {
            _matrix = new MatrixBase<Double>(doubleArray);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleArray"></param>
        public DoubleMatrix(MatrixBase<Double> matrix)
        {
            _matrix = matrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public DoubleMatrix(DoubleMatrix matrix) 
        {
            _matrix = matrix.InnerMatrix.Clone();
        }


        #endregion Public Constructors


        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        public DoubleMatrix Copy
        {
            get { return new DoubleMatrix(_matrix.Clone()); }
        }


        /// <summary>Gets or sets the value at the specified column and row of the generic matrix</summary>
        /// <param name="column">Column to access. Values are relative to the matrix view window</param>
        /// <param name="row">Row to access. Values are relative to the matrix view window</param>
        /// <returns>Value of the element at the specified column and row</returns>
        public virtual Double this[Int32 column, Int32 row]
        {
            get { return _matrix[column, row]; }
            set { _matrix[column, row] = value; }
        }

        /// <summary>Gets the number of columns visible within the matrix data view</summary>
        public virtual Int32 ColumnCount { get { return _matrix.ColumnCount; } }

        /// <summary>Gets a column accessor for this matrix, enabling column-based operations</summary>
        public virtual ColumnAccessor Columns { get { return new ColumnAccessor(this); } }

        /// <summary>Gets the range of columns visible within the matrix data view</summary>
        public virtual Int32Range DataColumnRange { get { return _matrix.DataColumnRange; } }

        /// <summary>Gets the range of rows visible within the matrix data view</summary>
        public virtual Int32Range DataRowRange { get { return _matrix.DataRowRange; } }

        /// <summary>Gets a reference to the data source underlying this matrix data view</summary>
        public virtual MatrixDataSource<Double> DataSource { get { return _matrix.DataSource; } }

        /// <summary>Gets a reference to the inner generic MatrixBase wrapped by this instance of DoubleMatrix</summary>
        public MatrixBase<Double> InnerMatrix { get { return _matrix; } }

        /// <summary>Gets a copy of the inverse of this square matrix</summary>
        public DoubleMatrix Inverse { get { return DoubleMatrix.Invert(this); } }

        /// <summary>Indicates whether this is matrix can be considered a column vector, where number of columns equals 1</summary>
        public virtual Boolean IsColumnVector { get { return _matrix.IsColumnVector; } }

        /// <summary>Indicates whether this matrix is empty (zero columns or zero rows)</summary>
        public virtual Boolean IsEmpty { get { return _matrix.IsEmpty; } }

        /// <summary>IsInvertible</summary>
        public Boolean IsInvertible { get { throw new NotImplementedException(); } }

        /// <summary>Indicates whether this matrix is not a vector (has > 1 row AND > 1 column)</summary>
        public virtual Boolean IsMatrix { get { return _matrix.IsMatrix; } }

        /// <summary>Indicates whether this is matrix can be considered a row vector, where number of rows equals 1</summary>
        public virtual Boolean IsRowVector { get { return _matrix.IsRowVector; } }


        /// <summary>
        /// Indicates whether this matrix represents a scalar value.
        /// A matrix can be considered representing a scalar value
        /// if number of columns equals 1 and number of rows equals 1
        /// NOTE: alternative definition: all elements = single value
        /// </summary>
        public virtual Boolean IsScalar { get { return _matrix.IsScalar;  } }

        /// <summary>Indicates whether this is a square matrix</summary>
        public virtual Boolean IsSquare { get { return _matrix.IsSquare; } }


        /// <summary>
        /// Indicates whether this matrix can be considered a vector. 
        /// For a given matrix of dimension m x n, it can be considered a vector
        /// if number of column equals 1 or number of rows equals 1
        /// </summary>
        public virtual Boolean IsVector { get { return _matrix.IsVector; } }

        /// <summary>Gets a copy of the pseudo-inverse of this matrix</summary>
        public DoubleMatrix PseudoInverse { get { return DoubleMatrix.PseudoInvert(this); } }

        /// <summary>Gets the number of rows visible within the matrix data view</summary>
        public virtual Int32 RowCount { get { return _matrix.RowCount; } }

        /// <summary>Gets a row accessor for this matrix, enabling row-based operations</summary>
        public virtual RowAccessor Rows { get { return new RowAccessor(this); } }

        /// <summary>Gets a transposed copy of this matrix (A^T)</summary>
        public virtual DoubleMatrix Transposed { get { return new DoubleMatrix(_matrix.Transposed); } }

        /// <summary>Gets the minimum value of all elements in the matrix</summary>
        public Double MinimumValue
        {
            get
            {
                if (this.IsEmpty) throw new ArithmeticException("Cannot calculate minimum value of an empty matrix");
                Double? minValue = null;
                for (int i = 0; i < this.ColumnCount; i++)
                    for (int j = 0; j < this.RowCount; j++)
                        if (minValue == null || (Double)minValue > this[i, j]) minValue = this[i, j];
                return (Double)minValue;
            }
        }

        /// <summary>Gets the maximum value of all elements in the matrix</summary>
        public Double MaximumValue
        {
            get
            {
                if (this.IsEmpty) throw new ArithmeticException("Cannot calculate maximum value of an empty matrix");
                Double? maxValue = null;
                for (int i = 0; i < this.ColumnCount; i++)
                    for (int j = 0; j < this.RowCount; j++)
                        if (maxValue == null || (Double)maxValue < this[i, j]) maxValue = this[i, j];
                return (Double)maxValue;
            }
        }


        #endregion Public Getters / Setters


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>Swaps the order of the two specified columns in the matrix</summary>
        /// <param name="row1">Column to swap</param>
        /// <param name="row2">Column to swap</param>
        public void SwapColumns(Int32 column1, Int32 column2)
        {
            MatrixBase<Double>.SwapColumns(this, column1, column2);
        }

        /// <summary>Swaps the order of the two specified rows in the matrix</summary>
        /// <param name="row1">Row to swap</param>
        /// <param name="row2">Row to swap</param>
        public void SwapRows(Int32 row1, Int32 row2)
        {
            MatrixBase<Double>.SwapRows(this, row1, row2);
        }


        /// <summary>CopyInto</summary>
        /// <param name="targetMatrix"></param>
        public void CopyInto(DoubleMatrix targetMatrix)
        {
            this.CopyInto(targetMatrix, 0, 0);
        }


        /// <summary>CopyInto</summary>
        /// <param name="targetMatrix"></param>
        /// <param name="targetColumnOffset"></param>
        /// <param name="targetRowOffset"></param>
        public void CopyInto(
            DoubleMatrix targetMatrix, 
            Int32 targetColumnOffset, 
            Int32 targetRowOffset)
        {
            MatrixBase<Double>.ElementWiseCopy(
                this.InnerMatrix, 
                targetMatrix.InnerMatrix, 
                targetColumnOffset, 
                targetRowOffset);
        }


        /// <summary>CopyFrom</summary>
        /// <param name="sourceMatrix"></param>
        public void CopyFrom(DoubleMatrix sourceMatrix)
        {
            this.InnerMatrix.CopyFrom(sourceMatrix.InnerMatrix, 0, 0);
        }


        /// <summary>CopyFrom</summary>
        /// <param name="sourceMatrix"></param>
        /// <param name="targetColumnOffset"></param>
        /// <param name="targetRowOffset"></param>
        public void CopyFrom(
            DoubleMatrix sourceMatrix, 
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            MatrixBase<Double>.ElementWiseCopy(
                sourceMatrix.InnerMatrix, 
                this.InnerMatrix, 
                targetColumnOffset, 
                targetRowOffset);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public DoubleMatrix CrossProduct(DoubleMatrix b)
        {
            throw new NotImplementedException();
            //return DoubleMatrix.CrossProduct(this, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Double DotProduct(DoubleMatrix b)
        {
            return DoubleMatrix.DotProduct(this, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix Negate()
        {
            return (-1) * this;
        }


        /// <summary>Normalizes the matrix so all values lie within the interval [0 .. 1]</summary>
        /// <returns>Normalized matrix</returns>
        public DoubleMatrix Normalize()
        {
            double min = this.MinimumValue;
            double max = this.MaximumValue;
            return ((this - min) / (max - min));
        }


        /// <summary>ScalarValue</summary>
        /// <returns></returns>
        public Double ScalarValue()
        {
            return DoubleMatrix.ScalarValue(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnRange"></param>
        /// <param name="rowRange"></param>
        /// <returns></returns>
        public DoubleMatrix SubMatrix(
            Int32Range columnRange,
            Int32Range rowRange)
        {
            return new DoubleMatrix(MatrixBase<Double>.SubMatrix(this.InnerMatrix, columnRange, rowRange));
        }


        public override string ToString()
        {
            return _matrix.ToString();
        }

        #endregion Public Methods


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>Indicates whether the specified Matrix is null or is empty (of rank 0)</summary>
        /// <param name="matrix"></param>
        /// <returns>True if specified Matrix is null, false otherwise</returns>
        public static Boolean IsNull(DoubleMatrix matrix)
        {
            return ((object)matrix == null);
        }


        /// <summary>Indicates whether the specified Matrix is null or is empty (of rank 0)</summary>
        /// <param name="matrix"></param>
        /// <returns>True if specified Matrix is null or empty (of rank == 0); false otherwise</returns>
        public static Boolean IsNullOrEmpty(DoubleMatrix matrix)
        {
            return ((object)matrix == null) || matrix.IsEmpty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static DoubleMatrix Addition(
            DoubleMatrix matrix1,
            DoubleMatrix matrix2)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate(Double element1, Double element2)
                    { return element1 + element2; }));
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static DoubleMatrix Addition(
            DoubleMatrix matrix,
            Double scalar)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(Double element1, Double element2)
                    { return element1 + element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static DoubleMatrix Subtraction(
            DoubleMatrix matrix1,
            DoubleMatrix matrix2)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate(double element1, double element2)
                    { return element1 - element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static DoubleMatrix Subtraction(
            DoubleMatrix matrix,
            double scalar)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 - element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static DoubleMatrix Division(
            DoubleMatrix matrix,
            double scalar)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 / element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static DoubleMatrix Multiplication(
            DoubleMatrix matrix,
            double scalar)
        {
            return new DoubleMatrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 * element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static DoubleMatrix Multiplication(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            if (matrix1.ColumnCount != matrix2.RowCount)
                throw new ArithmeticException("Number of columns in first matrix does not equal number of rows in second matrix.");

            DoubleMatrix result = new DoubleMatrix(matrix2.ColumnCount, matrix1.RowCount);

            for (int j = 0; j < result.RowCount; j++)
                for (int i = 0; i < result.ColumnCount; i++)
                {
                    Double value = 0;
                    for (int k = 0; k < matrix1.ColumnCount; k++)
                        value += matrix1[k, j] * matrix2[i, k];
                    result[i, j] = value;
                }

            return result;
        }


        /// <summary>Calculates the dot product (A . B) of two vectors</summary>
        /// <param name="vector1">First matrix (must be a row vector)</param>
        /// <param name="vector2">Second matrix (must be a column vector)</param>
        /// <returns>Returns the dot product of (matrix1 . matrix2)</returns>
        protected static Double DotProduct(DoubleMatrix vector1, DoubleMatrix vector2)
        {
            return Multiplication(vector1, vector2.Transposed).ScalarValue();
        }


        /// <summary>ScalarValue</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected static Double ScalarValue(DoubleMatrix matrix)
        {
            return matrix[0, 0];
        }

        /// <summary>Indicates whether or not two matricies contain same values
        /// in an element-wise comparison. Does not verify that the matricies point
        /// to the same underlying data-set or element instances</summary>
        /// <param name="matrix1">Matrix to compare</param>
        /// <param name="matrix2">Matrix to compare</param>
        /// <returns>true, if the matricies are equal on an element-wise basis; otherwise, false</returns>
        public static Boolean Equality(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            if ((object)matrix1 == null || (object)matrix2 == null) return false;
            return MatrixBase<Double>.Equality(matrix1.InnerMatrix, matrix2.InnerMatrix);
        }


        /// <summary>GetHashCode</summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.InnerMatrix.GetHashCode();
        }


        /// <summary>Equals</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            if (!(obj is DoubleMatrix)) return false;
            return this.InnerMatrix.Equals((obj as DoubleMatrix).InnerMatrix);
        }


        /// <summary>Generates a new identity matrix</summary>
        /// <param name="rank">Rank of the matrix (width and height)</param>
        /// <returns>Matrix of specified rank with all diagonalOffset elements set to 1, and all others set to 0</returns>
        public static DoubleMatrix Identity(int rank)
        {
            return DoubleMatrix.Identity(rank, rank);
        }

        /// <summary>Generates a new identity matrix</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <returns>Matrix of specified size with all diagonalOffset elements set to 1, and all others set to 0</returns>
        public static DoubleMatrix Identity(int columns, int rows)
        {
            DoubleMatrix identity = new DoubleMatrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    identity[i, j] = (i == j ? 1 : 0);
            return identity;
        }


        /// <summary>Generates a new zero matrix</summary>
        /// <param name="rank">Rank of the matrix</param>
        /// <returns>Matrix of specified rank with all element values set to 0</returns>
        public static DoubleMatrix Zero(int rank)
        {
            return DoubleMatrix.Zero(rank, rank);
        }


        /// <summary>Generates a new zero matrix</summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        /// <returns>Matrix of specified size with all element values set to 0</returns>
        public static DoubleMatrix Zero(int columns, int rows)
        {
            DoubleMatrix zero = new DoubleMatrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    zero[i, j] = 0;
            return zero;
        }


        /// <summary>Generates a new random matrix with element values in the range [0 .. 1]</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <returns>Matrix of specified size with random element values in range [0 .. 1]</returns>
        public static DoubleMatrix Random(int columns, int rows)
        {
            DoubleMatrix result = new DoubleMatrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    result[i, j] = _random.NextDouble();
            return result;
        }


        /// <summary>Calculates the inverse of a square matrix by applying
        /// Gaussian Elimination techniques. For more information on 
        /// algorithm see http://www.ecs.fullerton.edu/~mathews/numerical/mi.htm
        /// or your high school text book.</summary>
        /// <param name="matrix">Square matrix to be inverted</param>
        /// <returns>Inverse of input matrix</returns>
        protected static DoubleMatrix Invert(DoubleMatrix matrix)
        {

            if (!matrix.IsSquare) throw new ArithmeticException("Cannot invert non-square matrix");

            // create an augmented matrix [A,I] with the input matrix I on the 
            // left hand side and the identity matrix I on the right hand side
            //
            //    [ 2 5 6 | 1 0 0 ]
            // eg [ 8 3 1 | 0 1 0 ]
            //    [ 2 9 2 | 0 0 1 ]
            //
            DoubleMatrix augmentedMatrix =
                DoubleMatrix.JoinHorizontal(new DoubleMatrix[] { matrix, DoubleMatrix.Identity(matrix.ColumnCount, matrix.RowCount) });

            for (int j1 = 0; j1 < augmentedMatrix.RowCount; j1++)
            {

                // check to see if any of the rows subsequent to i have a 
                // higher absolute value on the current diagonalOffset (i,i).
                // if so, switch them to minimize rounding errors
                //
                //    [ (2) 5  6  | 1 0 0 ]                    [ (8) 3  1  | 0 1 0 ]
                // eg [  8 (3) 1  | 0 1 0 ] -> SWAP(R1, R2) -> [  2 (5) 6  | 1 0 0 ] 
                //    [  2  9 (2) | 0 0 1 ]                    [  2  9 (2) | 0 0 1 ]
                //
                for (int j2 = j1 + 1; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (Math.Abs(augmentedMatrix[j1, j2]) > Math.Abs(augmentedMatrix[j1, j1]))
                    {
                        //Console.WriteLine("Swap [" + j2 + "] with [" + i + "]");
                        augmentedMatrix.SwapRows(j1, j2);
                    }
                }

                // normalize the row so the diagonalOffset value (i,i) is 1
                // if (i,i) is 0, this row is null (we have > 0 nullity for this matrix)
                //
                //    [ (8) 3  1  | 0 1 0 ]                   [ (1.0) 0.4  0.1 | 0.0 0.1 0.0 ]
                // eg [  2 (5) 6  | 1 0 0 ] -> R1 = R1 / 8 -> [ 2.0  (5.0) 6.0 | 1.0 0.0 0.0 ] 
                //    [  2  9 (2) | 0 0 1 ]                   [ 2.0   9.0 (2.0) | 0.0 0.0 1.0 ]  

                //Console.WriteLine("Divide [" + i + "] by " + augmentedMatrix[i, i].ToString("0.00"));
                augmentedMatrix.Rows[j1].CopyFrom(augmentedMatrix.Rows[j1] / augmentedMatrix[j1, j1]);


                // look at each pair of rows {i, r} to see if r is linearly
                // dependent on i. if r does contain some factor of i vector,
                // subtract it out to make {i, r} linearly independent
                for (int j2 = 0; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (j2 != j1)
                    {
                        //Console.WriteLine("Subtracting " + augmentedMatrix[i, j2].ToString("0.00") + " i [" + i + "] from [" + j2 + "]");
                        augmentedMatrix.Rows[j2].CopyFrom(new DoubleMatrix(augmentedMatrix.Rows[j2] - (augmentedMatrix[j1, j2] * augmentedMatrix.Rows[j1])));
                    }
                }
            }

            // separate the inverse from the right hand side of the augmented matrix
            //
            //    [ (1) 0  0  |     [ 2 5 6 ] 
            // eg [  0 (1) 0  | ~   [ 8 2 1 ] -> inverse
            //    [  0  0 (1) |     [ 5 2 2 ] 
            //
            DoubleMatrix inverse = augmentedMatrix.SubMatrix(new Int32Range(matrix.ColumnCount, matrix.ColumnCount + matrix.ColumnCount), new Int32Range(0, matrix.RowCount));
            return inverse;
        }


        /// <summary>GaussianElimination</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static DoubleMatrix GaussianElimination(DoubleMatrix matrix)
        {
            DoubleMatrix reduced = matrix.Copy;

            Int32 lead = 0;
            for (Int32 row = 0; row < reduced.RowCount; row++)
            {
                if (reduced.ColumnCount <= lead)
                    break;

                Int32 i = row;
                while (reduced[lead, i] == 0)
                {
                    i++;
                    if (i == reduced.RowCount)
                    {
                        i = row;
                        lead++;
                        if (lead == reduced.ColumnCount) break;
                    }
                }
                reduced.SwapRows(i, row);
                reduced.Rows[row] = reduced.Rows[row] / reduced[lead, row];
                for (Int32 j = 0; j < reduced.RowCount; j++)
                {
                    if (j == row) continue;
                    reduced.Rows[j] = reduced.Rows[j] - reduced.Rows[row] * reduced[lead, j];
                }
            }
            return reduced;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected static DoubleMatrix PseudoInvert(DoubleMatrix matrix)
        {
            //                   (  T      )-1     T
            //  Pseudo inverse = ( A  i  A )   i  A
            //                   (         )
            return (matrix.Transposed * matrix).Inverse * matrix.Transposed;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static DoubleMatrix JoinHorizontal(IEnumerable<DoubleMatrix> matricies)
        {
            List<MatrixBase<Double>> innerMatrices = new List<MatrixBase<Double>>();
            foreach (DoubleMatrix matrix in matricies)
                innerMatrices.Add(matrix.InnerMatrix);
            return new DoubleMatrix(MatrixBase<Double>.JoinHorizontal(innerMatrices));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <returns></returns>
        public static DoubleMatrix JoinHorizontal(DoubleMatrix leftMatrix, DoubleMatrix rightMatrix)
        {
            return DoubleMatrix.JoinHorizontal(new DoubleMatrix[] { leftMatrix, rightMatrix } );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static DoubleMatrix JoinVertical(IEnumerable<DoubleMatrix> matricies)
        {
            List<MatrixBase<Double>> innerMatrices = new List<MatrixBase<Double>>();
            foreach (DoubleMatrix matrix in matricies)
                innerMatrices.Add(matrix.InnerMatrix);
            return new DoubleMatrix(MatrixBase<Double>.JoinVertical(innerMatrices));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="topMatrix"></param>
        /// <param name="bottomMatrix"></param>
        /// <returns></returns>
        public static DoubleMatrix JoinVertical(DoubleMatrix topMatrix, DoubleMatrix bottomMatrix)
        {
            return DoubleMatrix.JoinVertical(new DoubleMatrix[] { topMatrix, bottomMatrix });
        }


        #endregion Public Static Methods


        #region Public Operator Overloads
        /*****************************/
        /* PUBLIC OPERATOR OVERLOADS */
        /*****************************/

        public static DoubleMatrix operator +(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            return DoubleMatrix.Addition(matrix1, matrix2);
        }

        public static DoubleMatrix operator +(DoubleMatrix matrix, double scalar)
        {
            return DoubleMatrix.Addition(matrix, scalar);
        }

        public static DoubleMatrix operator +(double scalar, DoubleMatrix matrix)
        {
            return DoubleMatrix.Addition(matrix, scalar);
        }

        public static DoubleMatrix operator -(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            return DoubleMatrix.Subtraction(matrix1, matrix2);
        }

        public static DoubleMatrix operator -(DoubleMatrix matrix, double scalar)
        {
            return DoubleMatrix.Subtraction(matrix, scalar);
        }

        public static DoubleMatrix operator *(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            return DoubleMatrix.Multiplication(matrix1, matrix2);
        }

        public static DoubleMatrix operator *(double scalar, DoubleMatrix matrix)
        {
            return DoubleMatrix.Multiplication(matrix, scalar);
        }

        public static DoubleMatrix operator *(DoubleMatrix matrix, double scalar)
        {
            return DoubleMatrix.Multiplication(matrix, scalar);
        }

        public static DoubleMatrix operator /(DoubleMatrix matrix, double scalar)
        {
            return DoubleMatrix.Division(matrix, scalar);
        }

        public static bool operator ==(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            return DoubleMatrix.Equality(matrix1, matrix2);
        }

        public static bool operator !=(DoubleMatrix matrix1, DoubleMatrix matrix2)
        {
            return !DoubleMatrix.Equality(matrix1, matrix2);
        }

        #endregion Public Operator Overloads


        #region Public Implicit Operators
        /*****************************/
        /* PUBLIC IMPLICIT OPERATORS */
        /*****************************/

        /// <summary>DoubleMatrix</summary>
        /// <param name="doubleMatrix"></param>
        /// <returns></returns>
/*        public static implicit operator DoubleMatrix(MatrixBase<Double> doubleMatrix)
        {
            return new DoubleMatrix(doubleMatrix);
        }
        */

        /// <summary>DoubleMatrix</summary>
        /// <param name="doubleMatrix"></param>
        /// <returns></returns>
        public static implicit operator MatrixBase<Double>(DoubleMatrix doubleMatrix)
        {
            return doubleMatrix.InnerMatrix;
        }


        /// <summary>DoubleMatrix</summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        public static implicit operator DoubleMatrix(Double[,] dataArray)
        {
            return new DoubleMatrix(dataArray);
        }

        #endregion Public Implicit Operators

    }
}
