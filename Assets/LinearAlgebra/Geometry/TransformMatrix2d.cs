#region Header Comments
/*****************************************************************************/
/* TransformMatrix2d.cs                                                      */
/* -------------------------                                                 */
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
    public class TransformMatrix2d : DoubleMatrix
    {

        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        public TransformMatrix2d()
            : base(3)
        {
            this.CopyFrom(DoubleMatrix.Identity(3));
        }

        #endregion Public Constructors



        #region Public Accessors
        /********************/
        /* PUBLIC ACCESSORS */
        /********************/

        public DoubleMatrix CartesianTransformationMatrix
        {
            get { throw new NotImplementedException(); }
        }

        public DoubleMatrix CartesianTranslationVector
        {
            get { throw new NotImplementedException(); }
        }

        #endregion Public Accessors


        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Counterclockwise rotation angle in radians</param>
        /// <returns></returns>
        public static TransformMatrix2d RotationTransform(Double angleRadians)
        {
            TransformMatrix2d result = new TransformMatrix2d();
            result[0, 0] =  Math.Cos(angleRadians);
            result[1, 0] = -Math.Sin(angleRadians);
            result[0, 1] =  Math.Sin(angleRadians);
            result[1, 1] =  Math.Cos(angleRadians);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static TransformMatrix2d ScalingTransform(Double scale)
        {
            TransformMatrix2d result = new TransformMatrix2d();
            result[0, 0] = scale;
            result[1, 1] = scale;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shear"></param>
        /// <returns></returns>
        public static TransformMatrix2d HorizontalShearingTransform(Double shear)
        {
            TransformMatrix2d result = (TransformMatrix2d)DoubleMatrix.Identity(2);
            result[1, 0] = shear;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shear"></param>
        /// <returns></returns>
        public static TransformMatrix2d VerticalShearingTransform(Double shear)
        {
            TransformMatrix2d result = (TransformMatrix2d)DoubleMatrix.Identity(2);
            result[0, 1] = shear;
            return result;
        }


        public static Double FittingError(
            Point2dCollection sourcePoints,
            Point2dCollection targetPoints,
            TransformMatrix2d transform)
        {
            return 0;
        }


        public static TransformMatrix2d BestFitTransformation(
            Point2dCollection sourcePoints,
            Point2dCollection targetPoints)
        {
            throw new NotImplementedException();

            /*
            if (sourcePoints.Count != targetPoints.Count) throw new ArgumentException();

            DoubleMatrix c = new DoubleMatrix(3, 2);
            for (int j = 0; j < 2; j++)
                for (int i = 0; i < 3; i++)
                    for (int k = 0; k < sourcePoints.Count; k++)
                    {
                        DoubleMatrix qt = DoubleMatrix.JoinHorizontal(sourcePoints.Rows[k], DoubleMatrix.Identity(1));
                        c[i, j] += qt[i, 0] * targetPoints[j, k];
                    }

            DoubleMatrix Q = new DoubleMatrix(3, 3).Transposed;
            foreach (DoubleMatrix rowMatrix in sourcePoints.Rows)
            {
                DoubleMatrix qt = DoubleMatrix.JoinHorizontal(rowMatrix, DoubleMatrix.Identity(1));
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        Q[i, j] += qt[i, 0] * qt[j, 0];
                    }
            }

            DoubleMatrix M = DoubleMatrix.JoinVertical(Q, c).Transposed;

            DoubleMatrix reducedRow = DoubleMatrix.GaussianElimination(M);

            affineTransformationMatrix
                = reducedRow.SubMatrix(
                    new Int32Range(dimension + 1, 2 * dimension),
                    new Int32Range(0, dimension - 1));

            translationMatrix
                = reducedRow.SubMatrix(
                    new Int32Range(dimension + 1, 2 * dimension),
                    new Int32Range(dimension, dimension));
             */
        }


        #endregion Public Static Methods

    }
}
