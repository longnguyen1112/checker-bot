using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    class CheckerBox : PictureBox
    {
        private int _row;
        private int _col;
        private CheckerType _checker;

        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
            }
        }

        public int Col
        {
            get
            {
                return _col;
            }
            set
            {
                _col = value;
            }
        }

        public CheckerType Checker
        {
            get
            {
                return _checker;
            }
            set
            {
                _checker = value;
            }
        }
    }
}
