using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stagger.Objects
{
    class Set
    {
        private double _rightSize;
        private double _leftSize;
        private int _setCount;

        public int SetCount
        {
            get { return _setCount; }
            set { _setCount = value; }
        }
        
        public double LeftSize
        {
            get { return _leftSize; }
            set { _leftSize = value; }
        }

        public double RightSize
        {
            get { return _rightSize; }
            set { _rightSize = value; }
        }

        public double Stagger
        {
            get { return  Math.Round(RightSize - LeftSize ,1); }
        }
    }
}
