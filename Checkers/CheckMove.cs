using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    class CheckMove
    {
        private uint _red;
        private uint _black;
        private uint _king;
        private int _hVal;

        public uint Red
        {
            get
            {
                return _red;
            }
            set
            {
                _red = value;
            }
        }
        public uint Black
        {
            get
            {
                return _black;
            }
            set
            {
                _black = value;
            }
        }
        public uint King
        {
            get
            {
                return _king;
            }
            set
            {
                _king = value;
            }
        }

        public int  HeuristicValue
        {
            get
            {
                return _hVal;
            }
            set
            {
                _hVal = value;
            }
        }
    }
}
