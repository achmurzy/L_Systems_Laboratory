#region Header Comments
/*****************************************************************************/
/* Point2dCollection.cs                                                      */
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
    public class Point2dCollection : List<Point2d>
    {

        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        #endregion Public Constructors


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix AsAugmentedMatrix()
        {
            DoubleMatrix matrix = new DoubleMatrix(3, this.Count);
            for (int j = 0; j < this.Count; j++)
            {
                matrix[0, j] = this[j].AugmentedMatrix[0, 0];
                matrix[1, j] = this[j].AugmentedMatrix[1, 0];
                matrix[2, j] = this[j].AugmentedMatrix[2, 0];
            }
            return matrix;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DoubleMatrix AsMatrix()
        {
            DoubleMatrix matrix = new DoubleMatrix(2, this.Count);
            for (int j = 0; j < this.Count; j++)
            {
                matrix[0, j] = this[j].AugmentedMatrix[0, 0];
                matrix[1, j] = this[j].AugmentedMatrix[1, 0];
            }
            return matrix;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Point[] AsPointArray()
        {
            Point[] points = new Point[this.Count];
            for (int j = 0; j < this.Count; j++)
                points[j] = this[j].AsPoint();
            return points;
        }
         */

        #endregion Public Methods


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Point2dCollection FromMatrix(DoubleMatrix matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            if (matrix.ColumnCount != 3) throw new DimensionMismatchException();

            Point2dCollection collection = new Point2dCollection();
            foreach (DoubleMatrix row in matrix.Rows)
                collection.Add(new Point2d(row));

            return collection;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2dCollection FromPointArray(Point[] points)
        {
            if (points == null) throw new ArgumentNullException("points");

            Point2dCollection collection = new Point2dCollection();
            foreach (Point point in points)
                collection.Add(Point2d.FromPoint(point));

            return collection;
        }
        */
        #endregion Public Static Methods

    }
}
