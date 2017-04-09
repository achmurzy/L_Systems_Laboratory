#region Header Comments
/*****************************************************************************/
/* MatrixDataSource.cs                                                       */
/* -------------------                                                       */
/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* Comments and Notes                                                        */
/* ------------------                                                        */
/* Implementation of a generic data source provider for a matrix. At some    */
/* point in the future it would be good to abstract the matrix data source   */
/* interface.                                                                */
/*                                                                           */
/* This code is released to the public domain.                               */
/*****************************************************************************/
#endregion Header Comments

using System;

namespace LinearAlgebra.Matrices
{
    /// <summary>Generic matrix data provider</summary>
    /// <typeparam name="T">Element type of this data source</typeparam>
    public class MatrixDataSource<T>
    {

        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private T[,] _data;

        #endregion Private Members


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>Instatiate a new empty (0x0) matrix data source</summary>
        public MatrixDataSource() : this(0) { }


        /// <summary>Instantiate a new matrix data source of the specified rank (r x r)</summary>
        /// <param name="rank">Rank of the matrix (number of rows and number of columns)</param>
        public MatrixDataSource(Int32 rank) : this(rank, rank) { }


        /// <summary>Instantiate a new matrix data source of the specified number of rows and columns (c x r)</summary>
        /// <param name="columns">Number of columns in the data source</param>
        /// <param name="rows">Number of rows in the data source</param>
        public MatrixDataSource(Int32 columns, Int32 rows)
        {
            _data = new T[columns, rows];
        }

        /// <summary>Instantiate a new matrix data source with using the provided 2 dimensional data array</summary>
        /// <param name="dataArray">Two-dimensional array of data to use when instantiating the matrix</param>
        public MatrixDataSource(T[,] dataArray)
        {
            // row first, translate it into column first representation
            Int32 rowCount = dataArray.GetLength(0);
            Int32 columnCount = dataArray.GetLength(1);
            _data = new T[columnCount, rowCount];
            for (Int32 i = 0; i < columnCount; i++)
                for (Int32 j = 0; j < rowCount; j++)
                    _data[i, j] = dataArray[j, i];
        }

        #endregion Public Constructors


        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        /// <summary>Gets or sets the data source value at the specified column and row</summary>
        public T this[Int32 column, Int32 row]
        {
            get { return _data[column, row]; }
            set { _data[column, row] = value; }
        }

        /// <summary>Gets the number of columns in this matrix data source</summary>
        public Int32 ColumnCount { get { return _data.GetLength(0); } }

        /// <summary>Gets the number of rows in this matrix data source</summary>
        public Int32 RowCount { get { return _data.GetLength(1); } }

        /// <summary>Gets the default matrix data view including all columns and rows in this data source</summary>
        public MatrixBase<T> DefaultView { get { return new MatrixBase<T>(this); } }

        #endregion Public Getters / Setters

    }
}
