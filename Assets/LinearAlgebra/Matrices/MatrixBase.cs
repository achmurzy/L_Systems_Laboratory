#region Header Comments
/*****************************************************************************/
/* MatrixBase.cs                                                             */
/* -------------                                                             */
/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* Comments and Notes                                                        */
/* ------------------                                                        */
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
    /// <summary>Generic matrix class. A MatrixBase instance acts as an accessor 
    /// to provide a window into a MatrixDataSource class. A MatrixBase can be used 
    /// to create a 'window' into an existing data source, creating a subset view</summary>
    /// <typeparam name="T">Element type of this matrix</typeparam>
    public class MatrixBase<T> 
    {

        public class RowAccessor<T>
            : IEnumerable<MatrixBase<T>>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private MatrixBase<T> _dataView;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>RowAccessor</summary>
            /// <param name="dataView"></param>
            public RowAccessor(MatrixBase<T> dataView)
            {
                _dataView = dataView;
            }
            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            public MatrixBase<T> this[Int32 rowIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(rowIndex);
                }
                set
                {
                    if (value.ColumnCount != _dataView.ColumnCount || value.RowCount != 1)
                        throw new DimensionMismatchException();

                    MatrixBase<T> targetRow = this.AsEnumerable().ElementAt(rowIndex);
                    for (int i = 0; i < value.ColumnCount; i++)
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
            public IEnumerable<MatrixBase<T>> AsEnumerable()
            {
                int j = 0;
                while (j < _dataView.RowCount)
                    yield return _dataView.SubMatrix(new Int32Range(0, _dataView.ColumnCount), new Int32Range(j, ++j));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<MatrixBase<T>> GetEnumerator()
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

        public class ColumnAccessor<T>
            : IEnumerable<MatrixBase<T>>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private MatrixBase<T> _dataView;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>ColumnAccessor</summary>
            /// <param name="dataView"></param>
            public ColumnAccessor(MatrixBase<T> dataView)
            {
                _dataView = dataView;
            }

            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            /// <summary>this</summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public MatrixBase<T> this[Int32 columnIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(columnIndex);
                }
                set
                {
                    if (value.RowCount != _dataView.RowCount || value.ColumnCount != 1)
                        throw new DimensionMismatchException();

                    MatrixBase<T> targetColumn = this.AsEnumerable().ElementAt(columnIndex);
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
            public IEnumerable<MatrixBase<T>> AsEnumerable()
            {
                int i = 0;
                while (i < _dataView.ColumnCount)
                    yield return _dataView.SubMatrix(new Int32Range(i, ++i), new Int32Range(0, _dataView.RowCount));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<MatrixBase<T>> GetEnumerator()
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


        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private Int32Range _dataColumnRange;
        private Int32Range _dataRowRange;
        private MatrixDataSource<T> _dataSource;

        #endregion Private Members


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>Instatiates an empty (0x0) generic matrix with a new underlying matrix data source</summary>
        public MatrixBase() : this(0) { }


        /// <summary>Instatiates a square generic matrix of the specified rank (rank x rank) with a new underlying matrix data source</summary>
        /// <param name="rank">The rank of the square generic matrix</param>
        public MatrixBase(Int32 rank) : this(rank, rank) { }


        /// <summary>Instatiates a generic matrix of the specified dimensions (columns x rows) with a new underlying matrix data source</summary>
        /// <param name="columns">Number of columns in the matrix</param>
        /// <param name="rows">Number of rows in the matrix</param>
        public MatrixBase(Int32 columns, Int32 rows)
        {
            _dataSource = new MatrixDataSource<T>(columns, rows);
            _dataRowRange = new Int32Range(0, _dataSource.RowCount);
            _dataColumnRange = new Int32Range(0, _dataSource.ColumnCount);
        }


        /// <summary>Instantiates a generic matrix populated with the elements of the provided elementArray</summary>
        /// <param name="elementArray">Initial matrix values (encoded row-first)</param>
        public MatrixBase(T[,] elementArray)
        {
            _dataSource = new MatrixDataSource<T>(elementArray);
            _dataRowRange = new Int32Range(0, _dataSource.RowCount);
            _dataColumnRange = new Int32Range(0, _dataSource.ColumnCount);
        }


        /// <summary>Instantiates a generic matrix populated with the elements of the provides matrix</summary>
        /// <param name="matrix">Matrix containing the initial values</param>
        public MatrixBase(MatrixBase<T> matrix)
            : this(matrix.ColumnCount, matrix.RowCount)
        {
            this.CopyFrom(matrix);
        }


        /// <summary>Instatiates a generic matrix with the provided matrix data source (all columns and rows)</summary>
        /// <param name="matrixData">Matrix data source</param>
        public MatrixBase(MatrixDataSource<T> matrixData)
        {
            _dataSource = matrixData;
            _dataRowRange = new Int32Range(0, matrixData.RowCount);
            _dataColumnRange = new Int32Range(0, matrixData.ColumnCount);
        }


        /// <summary>Instatiates a general matrix with the provided matrix data source, 
        /// limited to the specified column and row ranges</summary>
        /// <param name="matrixData">Matrix data source</param>
        /// <param name="columnRange">Visible column range</param>
        /// <param name="rowRange">Visible row range</param>
        protected MatrixBase(
            MatrixDataSource<T> matrixData,
            Int32Range columnRange,
            Int32Range rowRange)
        {
            _dataSource = matrixData;
            _dataColumnRange = columnRange;
            _dataRowRange = rowRange;
        }

        #endregion Public Constructors


        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        /// <summary>Gets or sets the value at the specified column and row of the generic matrix</summary>
        /// <param name="column">Column to access. Values are relative to the matrix view window</param>
        /// <param name="row">Row to access. Values are relative to the matrix view window</param>
        /// <returns>Value of the element at the specified column and row</returns>
        public virtual T this[Int32 column, Int32 row]
        {
            get
            {
                if (!_dataColumnRange.Contains(column + _dataColumnRange.Start)) throw new ArgumentOutOfRangeException("column");
                if (!_dataRowRange.Contains(row + _dataRowRange.Start)) throw new ArgumentOutOfRangeException("row");
                return _dataSource[column + _dataColumnRange.Start, row + _dataRowRange.Start];
            }
            set
            {
                if (!_dataColumnRange.Contains(column + _dataColumnRange.Start)) throw new ArgumentOutOfRangeException("column");
                if (!_dataRowRange.Contains(row + _dataRowRange.Start)) throw new ArgumentOutOfRangeException("row");
                _dataSource[column + _dataColumnRange.Start, row + _dataRowRange.Start] = value;
            }
        }

        /// <summary>Gets the number of columns visible within the matrix data view</summary>
        public virtual Int32 ColumnCount { get { return _dataColumnRange.Length; } }

        /// <summary>Gets a column accessor for this matrix, enabling column-based operations</summary>
        public virtual ColumnAccessor<T> Columns { get { return new ColumnAccessor<T>(this); } }

        /// <summary>Gets a cloned copy of this matrix</summary>
        public virtual MatrixBase<T> Copy { get { return this.Clone(); } }

        /// <summary>Gets the range of columns visible within the matrix data view</summary>
        public virtual Int32Range DataColumnRange { get { return _dataColumnRange; } }

        /// <summary>Gets the range of rows visible within the matrix data view</summary>
        public virtual Int32Range DataRowRange { get { return _dataRowRange; } }

        /// <summary>Gets a reference to the data source underlying this matrix data view</summary>
        public virtual MatrixDataSource<T> DataSource { get { return _dataSource; } }

        /// <summary>Indicates whether this is matrix can be considered a column vector, where number of columns equals 1</summary>
        public virtual Boolean IsColumnVector { get { return !this.IsEmpty && this.ColumnCount == 1; } }

        /// <summary>Indicates whether this matrix is empty (zero columns or zero rows)</summary>
        public virtual Boolean IsEmpty { get { return this.RowCount == 0 || this.ColumnCount == 0; } }

        /// <summary>Indicates whether this matrix is not a vector (has > 1 row AND > 1 column)</summary>
        public virtual Boolean IsMatrix { get { return this.RowCount > 1 && this.ColumnCount > 1; } }

        /// <summary>Indicates whether this is matrix can be considered a row vector, where number of rows equals 1</summary>
        public virtual Boolean IsRowVector { get { return !this.IsEmpty && this.RowCount == 1; } }


        /// <summary>
        /// Indicates whether this matrix represents a scalar value.
        /// A matrix can be considered representing a scalar value
        /// if number of columns equals 1 and number of rows equals 1
        /// NOTE: alternative definition: all elements = single value
        /// </summary>
        public virtual Boolean IsScalar { get { return this.RowCount == 1 && this.ColumnCount == 1; } }

        /// <summary>Indicates whether this is a square matrix</summary>
        public virtual Boolean IsSquare { get { return this.RowCount == this.ColumnCount; } }


        /// <summary>
        /// Indicates whether this matrix can be considered a vector. 
        /// For a given matrix of dimension m x n, it can be considered a vector
        /// if number of column equals 1 or number of rows equals 1
        /// </summary>
        public virtual Boolean IsVector { get { return this.IsRowVector || this.IsColumnVector; } }

        /// <summary>Gets the number of rows visible within the matrix data view</summary>
        public virtual Int32 RowCount { get { return _dataRowRange.Length; } }

        /// <summary>Gets a row accessor for this matrix, enabling row-based operations</summary>
        public virtual RowAccessor<T> Rows { get { return new RowAccessor<T>(this); } }

        /// <summary>Gets a transposed copy of this matrix (A^T)</summary>
        public virtual MatrixBase<T> Transposed { get { return MatrixBase<T>.Transpose(this); } }

        #endregion Public Getters / Setters


        #region Public Delegates
        /********************/
        /* PUBLIC DELEGATES */
        /********************/
        public delegate T ElementUnaryOperationDelegate(T element);
        public delegate T ElementBinaryOperationDelegate(T element1, T element2);
        public delegate T ElementPositionalUnaryOperationDelegate(T element, Int32 column, Int32 row);
        public delegate T ElementPositionalBinaryOperationDelegate(T element1, T element2, Int32 column, Int32 row);

        #endregion Public Delegates


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>Creates a deep copy of this matrix object (with a copy
        /// of the underlying data source)</summary>
        /// <returns>Matrix instance</returns>
        public virtual MatrixBase<T> Clone()
        {
            return new MatrixBase<T>(this);
        }


        /// <summary>Copies the values of the matrix object into
        /// a target matrix of the same size.</summary>
        /// <param name="targetMatrix">Target matrix</param>
        public virtual void CopyInto(MatrixBase<T> targetMatrix)
        {
            this.CopyInto(targetMatrix, 0, 0);
        }


        /// <summary>Copies the values of the matrix object into a 
        /// larger matrix at the specified offset</summary>
        /// <param name="matrix">Target matrix</param>
        /// <param name="targetColumnOffset">Target matrix column offset</param>
        /// <param name="targetRowOffset">Target matrix row offset</param>
        public virtual void CopyInto(
            MatrixBase<T> targetMatrix,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            MatrixBase<T>.ElementWiseCopy(this, targetMatrix, targetColumnOffset, targetRowOffset);
        }


        /// <summary>Copies the values of the specified source matrix into
        /// this matrix (of the same size)</summary>
        /// <param name="sourceMatrix">Source matrix</param>
        public virtual void CopyFrom(MatrixBase<T> sourceMatrix)
        {
            this.CopyFrom(sourceMatrix, 0, 0);
        }


        /// <summary>Copies the values of the specified source matrix
        /// into this (larger) matrix at the specified offset</summary>
        /// <param name="sourceMatrix">Source matrix</param>
        /// <param name="targetColumnOffset">Target column offset in this matrix</param>
        /// <param name="targetRowOffset">Target row offset in this matrix</param>
        public virtual void CopyFrom(
            MatrixBase<T> sourceMatrix,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            MatrixBase<T>.ElementWiseCopy(sourceMatrix, this, targetColumnOffset, targetRowOffset);
        }


        /// <summary>Determines whether two MatrixBase instances are equal</summary>
        /// <param name="obj">The MatrixBase object to compare with the current MatrixBase object</param>
        /// <returns>true if the specified MatrixBase object is equal to the curent MatrixBase object; otherwise, false</returns>
        public override Boolean Equals(Object obj)
        {
            return MatrixBase<T>.Equality(this, obj as MatrixBase<T>);
        }


        /// <summary>Creates a new matrix pointing at the same underlying data
        /// set, windowed to the specified column and row ranges</summary>
        /// <param name="columnRange">Sub-matrix column range</param>
        /// <param name="rowRange">Sub-matrix row range</param>
        /// <returns></returns>
        public virtual MatrixBase<T> SubMatrix(
            Int32Range columnRange,
            Int32Range rowRange)
        {
            return MatrixBase<T>.SubMatrix(this, columnRange, rowRange);
        }


        /// <summary>Swaps the order of the two specified columns in the matrix</summary>
        /// <param name="row1">Column to swap</param>
        /// <param name="row2">Column to swap</param>
        public virtual void SwapColumns(Int32 column1, Int32 column2)
        {
            MatrixBase<T>.SwapColumns(this, column1, column2);
        }


        /// <summary>Swaps the order of the two specified rows in the matrix</summary>
        /// <param name="row1">Row to swap</param>
        /// <param name="row2">Row to swap</param>
        public virtual void SwapRows(Int32 row1, Int32 row2)
        {
            MatrixBase<T>.SwapRows(this, row1, row2);
        }



        /// <summary>Returns a String that represents the current MatrixBase object</summary>
        /// <returns>Returns a String that represents the current MatrixBase object</returns>
        public override String ToString()
        {
            String result = String.Empty;
            if (this.IsEmpty) return "[empty]";

            result += String.Format("{0} columns x {1} rows\n", this.ColumnCount, this.RowCount);

            for (Int32 j = 0; j < this.RowCount; j++)
            {
                String rowStr = String.Empty;
                for (Int32 i = 0; i < this.ColumnCount; i++)
                    rowStr += String.Format(" {0} ", this[i, j].ToString());
                result += String.Format("[{0}]\n", rowStr);
            }
            return result;
        }

        #endregion Public Methods


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>Generates a new default matrix</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <returns>Matrix of specified size with all element values set to 0</returns>
        public static MatrixBase<T> Default(Int32 columns, Int32 rows)
        {
            MatrixBase<T> defaultMatrix = new MatrixBase<T>(columns, rows);
            for (Int32 i = 0; i < columns; i++)
                for (Int32 j = 0; j < rows; j++)
                    defaultMatrix[i, j] = default(T);
            return defaultMatrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Boolean DimensionsMatch(
            MatrixBase<T> matrix1, 
            MatrixBase<T> matrix2)
        {
            if (matrix1 == null) throw new ArgumentNullException("matrix1");
            if (matrix2 == null) throw new ArgumentNullException("matrix2");
            return matrix1.ColumnCount == matrix2.ColumnCount
                   && matrix1.RowCount == matrix2.RowCount;
        }


        /// <summary>Performs element-wise copy of the source matrix into 
        /// the target matrix at the specified offset</summary>
        /// <param name="sourceMatrix">Source matrix</param>
        /// <param name="targetMatrix">Target matrix</param>
        /// <param name="targetColumnOffset">Target column offset</param>
        /// <param name="targetRowOffset">Target row offset</param>
        public static void ElementWiseCopy(
            MatrixBase<T> source,
            MatrixBase<T> target,
            Int32 targetColumnOffset,
            Int32 targetRowOffset)
        {
            for (Int32 i = 0; i < source.ColumnCount; i++)
                for (Int32 j = 0; j < source.RowCount; j++)
                    target[i + targetColumnOffset, j + targetRowOffset] = source[i, j];
        }


        /// <summary>Performs an element-wise binary operation (involving two matricies),  
        /// returning the result in a new matrix instance</summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <param name="operation">Binary operation delegate</param>
        /// <returns>Matrix instance containing the result of the binary operation</returns>
        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix1,
            MatrixBase<T> matrix2,
            ElementBinaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix1)) throw new ArgumentNullException("matrix1");
            if (MatrixBase<T>.IsNull(matrix2)) throw new ArgumentNullException("matrix2");
            if (matrix1.ColumnCount != matrix2.ColumnCount) throw new DimensionMismatchException("The number of columns in matrix1 does not equal the number of columns in matrix2");
            if (matrix1.RowCount != matrix2.RowCount) throw new DimensionMismatchException("The number of rows in matrix1 does not equal the number of rows in matrix2");

            MatrixBase<T> result = new MatrixBase<T>(matrix1.ColumnCount, matrix1.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix1[i, j], matrix2[i, j]);
            return result;
        }


        /// <summary>Performs an element-wise binary operation involving a matrix and a scalar,  
        /// returning the result in a new matrix instance</summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="scalar">Scalar</param>
        /// <param name="operation">Binary operation delegate</param>
        /// <returns>Matrix instance containing the result of the binary operation</returns>
        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix,
            T scalar,
            ElementBinaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException("matrix");

            MatrixBase<T> result = new MatrixBase<T>(matrix.ColumnCount, matrix.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix[i, j], scalar);
            return result;
        }


        /// <summary>Performs an element-wise unary operation involving a single matrix,   
        /// returning the result in a new matrix instance</summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="operation">Unary operation delegate</param>
        /// <returns>Matrix instance containing the result of the unary operation</returns>
        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix,
            ElementUnaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException("matrix");

            MatrixBase<T> result = new MatrixBase<T>(matrix.ColumnCount, matrix.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix[i, j]);
            return result;
        }


        /// <summary>Performs an element-wise unary operation involving a single matrix,   
        /// returning the result in a new matrix instance</summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="operation">Unary operation delegate</param>
        /// <returns>Matrix instance containing the result of the unary operation</returns>
        public static MatrixBase<T> ElementWiseOperation(
            MatrixBase<T> matrix,
            ElementPositionalUnaryOperationDelegate operation)
        {
            if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException("matrix");

            MatrixBase<T> result = new MatrixBase<T>(matrix.ColumnCount, matrix.RowCount);
            for (Int32 i = 0; i < result.ColumnCount; i++)
                for (Int32 j = 0; j < result.RowCount; j++)
                    result[i, j] = operation(matrix[i, j], i, j);
            return result;
        }


        /// <summary>Indicates whether or not two matricies contain same values
        /// in an element-wise comparison. Does not verify that the matricies point
        /// to the same underlying data-set or element instances</summary>
        /// <param name="matrix1">Matrix to compare</param>
        /// <param name="matrix2">Matrix to compare</param>
        /// <returns>true, if the matricies are equal on an element-wise basis; otherwise, false</returns>
        public static Boolean Equality(
            MatrixBase<T> matrix1, 
            MatrixBase<T> matrix2)
        {
            if ((Object)matrix1 == null || (Object)matrix2 == null) return false;
            if (matrix1.ColumnCount != matrix2.ColumnCount) return false;
            if (matrix1.RowCount != matrix2.RowCount) return false;

            for (Int32 i = 0; i < matrix1.ColumnCount; i++)
                for (Int32 j = 0; j < matrix1.RowCount; j++)
                    if (!matrix1[i, j].Equals(matrix2[i, j])) return false;

            return true;
        }


        /// <summary>Indicates whether the specified matrix is null</summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>true, if specified matrix is null; otherwise, false</returns>
        public static Boolean IsNull(MatrixBase<T> matrix)
        {
            return ((object)matrix == null);
        }


        /// <summary>Indicates whether the specified Matrix is null or is empty (of rank 0)</summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>true, if specified matrix is null or empty (of rank == 0); otherwise, false</returns>
        public static Boolean IsNullOrEmpty(MatrixBase<T> matrix)
        {
            return ((object)matrix == null) || matrix.IsEmpty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static MatrixBase<T> JoinHorizontal(
            IEnumerable<MatrixBase<T>> matricies)
        {
            int? rowCount = null;
            int totalColumnCount = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException();
                if (rowCount == null) rowCount = matrix.RowCount;
                else if ((int)rowCount != matrix.RowCount) throw new DimensionMismatchException();
                totalColumnCount += matrix.ColumnCount;
            }

            MatrixBase<T> resultMatrix = new MatrixBase<T>(totalColumnCount, (int)rowCount);

            int columnOffset = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                MatrixBase<T>.ElementWiseCopy(matrix, resultMatrix, columnOffset, 0);
                columnOffset += matrix.ColumnCount;
            }

            return resultMatrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <returns></returns>
        public static MatrixBase<T> JoinHorizontal(
            MatrixBase<T> leftMatrix, 
            MatrixBase<T> rightMatrix)
        {
            return MatrixBase<T>.JoinHorizontal(new MatrixBase<T>[] { leftMatrix, rightMatrix });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static MatrixBase<T> JoinVertical(
            IEnumerable<MatrixBase<T>> matricies)
        {
            int? columnCount = null;
            int totalRowCount = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                if (MatrixBase<T>.IsNull(matrix)) throw new ArgumentNullException();
                if (columnCount == null) columnCount = matrix.ColumnCount;
                else if ((int)columnCount != matrix.ColumnCount) throw new DimensionMismatchException();
                totalRowCount += matrix.RowCount;
            }

            MatrixBase<T> resultMatrix = new MatrixBase<T>((int)columnCount, totalRowCount);

            int rowOffset = 0;
            foreach (MatrixBase<T> matrix in matricies)
            {
                MatrixBase<T>.ElementWiseCopy(matrix, resultMatrix, 0, rowOffset);
                rowOffset += matrix.RowCount;
            }

            return resultMatrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="topMatrix"></param>
        /// <param name="bottomMatrix"></param>
        /// <returns></returns>
        public static MatrixBase<T> JoinVertical(
            MatrixBase<T> topMatrix, 
            MatrixBase<T> bottomMatrix)
        {
            return MatrixBase<T>.JoinVertical(new MatrixBase<T>[] { topMatrix, bottomMatrix });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="columnRange"></param>
        /// <param name="rowRange"></param>
        /// <returns></returns>
        public static MatrixBase<T> SubMatrix(
            MatrixBase<T> view,
            Int32Range columnRange,
            Int32Range rowRange)
        {
            if (!view.DataColumnRange.Contains(columnRange)) throw new ArgumentOutOfRangeException("columnRange");
            if (!view.DataRowRange.Contains(rowRange)) throw new ArgumentOutOfRangeException("rowRange");

            MatrixBase<T> result
                = new MatrixBase<T>(view.DataSource,
                       new Int32Range(
                           columnRange.Start + view.DataColumnRange.Start,
                           columnRange.End + view.DataColumnRange.Start),
                       new Int32Range(
                           rowRange.Start + view.DataRowRange.Start,
                           rowRange.End + view.DataRowRange.Start));
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        public static void SwapColumns(
            MatrixBase<T> matrix,
            Int32 column1,
            Int32 column2)
        {
            MatrixBase<T> columnTemp = matrix.Columns[column1].Clone();
            matrix.Columns[column1] = matrix.Columns[column2];
            matrix.Columns[column2] = columnTemp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        public static void SwapRows(
            MatrixBase<T> matrix, 
            Int32 row1, 
            Int32 row2)
        {
            MatrixBase<T> rowTemp = matrix.Rows[row1].Clone();
            matrix.Rows[row1] = matrix.Rows[row2];
            matrix.Rows[row2] = rowTemp;
        }


        /// <summary>Transposes a matrix (reflecting it across the diagonalOffset axis)
        /// swapping all elements (i, i) with elements (i, i).</summary>
        /// <param name="matrix">Matrix to be transposed</param>
        /// <returns>Returns a transposed copy of the input matrix</returns>
        public static MatrixBase<T> Transpose(MatrixBase<T> matrix)
        {
            MatrixBase<T> result
                = new MatrixBase<T>(matrix.RowCount, matrix.ColumnCount);

            for (Int32 i = 0; i < matrix.ColumnCount; i++)
                for (Int32 j = 0; j < matrix.RowCount; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }


        #endregion Public Static Methods


        #region Public Operator Overloads
        /*****************************/
        /* PUBLIC OPERATOR OVERLOADS */
        /*****************************/

        /// <summary>==</summary>
        /// <param name="matrix1">Left-hand matrix</param>
        /// <param name="matrix2">Right-hand matrix</param>
        /// <returns></returns>
        public static Boolean operator ==(MatrixBase<T> matrix1, MatrixBase<T> matrix2)
        {
            return MatrixBase<T>.Equality(matrix1, matrix2);
        }

        /// <summary>!=</summary>
        /// <param name="matrix1">Left-hand matrix</param>
        /// <param name="matrix2">Right-hand matrix</param>
        /// <returns></returns>
        public static Boolean operator !=(MatrixBase<T> matrix1, MatrixBase<T> matrix2)
        {
            return !MatrixBase<T>.Equality(matrix1, matrix2);
        }

        /// <summary>GetHashCode</summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Public Operator Overloads


        #region Public Implicit Operators
        /*****************************/
        /* PUBLIC IMPLICIT OPERATORS */
        /*****************************/

        public static implicit operator MatrixBase<T>(T[,] dataArray)
        {
            return new MatrixBase<T>(dataArray);
        }

        #endregion Public Implicit Operators

    }
}
