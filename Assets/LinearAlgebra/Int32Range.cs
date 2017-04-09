#region Header Comments
/*****************************************************************************/
/* Int32Range.cs                                                             */
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
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra
{
    public class Int32Range
    {

        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private Int32 _end;
        private Int32 _start;

        #endregion Private Members


        #region Public Accessors
        /********************/
        /* PUBLIC ACCESSORS */
        /********************/

        public Int32 End
        {
            get { return _end; }
            set { _end = value; }
        }

        public Int32 Length
        {
            get { return _end - _start; }
        }

        public Int32 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        #endregion Public Accessors


        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>Int32Range</summary>
        /// <param name="start"></param>
        public Int32Range(Int32 start) : this(start, start) { }


        /// <summary>Int32Range</summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Int32Range(Int32 start, Int32 end)
        {
            _start = Math.Min(start, end);
            _end = Math.Max(start, end);
        }

        #endregion Public Constructors


        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>Contains</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean Contains(Int32 value)
        {
            return value >= _start && value < _end;
        }


        /// <summary>Contains</summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public Boolean Contains(Int32Range range)
        {
            return range.Start >= _start && range.End <= _end;
        }

        /// <summary>ToString</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} [{1} - {2}]\n", this.GetType().Name, _start, _end);
        }

        #endregion Public Methods
    }
}
