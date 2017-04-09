#region Header Comments
/*****************************************************************************/
/* Point2d.cs                                                                */
/* -------------------                                                       */
/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* This code is released to the public domain.                               */
/*****************************************************************************/
#endregion Header Comments

using System;
using System.Collections.Generic;
using System.Linq;
using LinearAlgebra.Matrices;

namespace LinearAlgebra.Geometry
{
    public class Point2d
    {

        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private DoubleMatrix _matrix;

        #endregion Private Members


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/
        /// <summary>Instantiates a new 2d point with the specified x and y 
        /// coordinates. Creates a new underlying matrix data source</summary>
        /// <param name="x">x-coordinate of this 2d point</param>
        /// <param name="y">y-coordinate of this 2d point</param>
        public Point2d(Double x, Double y)
        {
            _matrix = new DoubleMatrix(3, 1);
            _matrix[0, 0] = x;
            _matrix[1, 0] = y;
            _matrix[2, 0] = 1F;
        }


        /// <summary>Instantiates a new 2d point using the provided augmented vector
        /// matrix as the underlying data source</summary>
        /// <param name="augmentedMatrix">Augmented 3x1 matrix containing the point coordinates</param>
        public Point2d(DoubleMatrix augmentedMatrix)
        {
            if (augmentedMatrix == null) throw new ArgumentNullException("augmentedMatrix");
            if (augmentedMatrix.RowCount != 1) throw new DimensionMismatchException();
            if (augmentedMatrix.ColumnCount != 3) throw new DimensionMismatchException();
            if (augmentedMatrix[2, 0] != 1) throw new ArgumentException("Point matrix does not appear to be an augmented 3x1 2-d point matrix in form [x, y, 1]", "augmentedMatrix");
            _matrix = augmentedMatrix;
        }

        #endregion Public Constructors


        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        /// <summary>Gets a reference to the 3x1 augmented 
        /// coordinate vector of this 2d point</summary>
        public DoubleMatrix AugmentedMatrix
        {
            get { return _matrix; }
        }

        /// <summary>Gets a reference to the 2x1 non-augmented 
        /// coordinate vector of this 2d point</summary>
        public DoubleMatrix Matrix
        {
            get
            {
                return _matrix.SubMatrix(new Int32Range(0, 2), new Int32Range(0, 1));
            }
        }

        /// <summary>Gets or sets the x-coordinate of this 2d point</summary>
        public Double X
        {
            get { return _matrix[0, 0]; }
            set { _matrix[0, 0] = value; }
        }

        /// <summary>Gets or sets the y-coordinate of this 2d point</summary>
        public Double Y
        {
            get { return _matrix[1, 0]; }
            set { _matrix[1, 0] = value; }
        }

        #endregion Public Accessors


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /*
        /// <summary>Returns a copy of this Point2d as a System.Windows.Point structure</summary>
        /// <returns>System.Windows.Point copy of this Point2d</returns>
        public Point AsPoint()
        {
            return new Point(this.X, this.Y);
        }
        */

        /// <summary>Returns a string representation of this Point2d object</summary>
        /// <returns>string representation of this Point2d object</returns>
        public override string ToString()
        {
            return String.Format("{0} - ({1}, {2})", this.GetType(), this.X, this.Y);
        }
        #endregion Public Methods


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /*
        /// <summary>Returns a new Point2d object containing the coordinates of the 
        /// provided System.Windows.Point struct</summary>
        /// <param name="point">Point structure containing the initial (x,y) coordinates</param>
        /// <returns>New Point2d object for the specified coordinates</returns>
        public static Point2d FromPoint(Point point)
        {
            return new Point2d(point.X, point.Y);
        }
        */

        #endregion


    }
}
