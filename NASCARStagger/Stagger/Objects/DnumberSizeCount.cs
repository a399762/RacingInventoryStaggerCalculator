using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stagger.Objects
{
    class DnumberSizeCount
    {
        private double _size;
        private int _count;
        private int _used;
        private int _tempUsed;

        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public int Used
        {
            get { return _used; }
            set { _used = value; }
        }

        public int TempUsed
        {
            get { return _tempUsed; }
            set { _tempUsed = value; }
        }  

    }
}
