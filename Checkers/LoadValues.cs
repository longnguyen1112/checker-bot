using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{
    public partial class LoadValues : Form
    {
        public LoadValues()
        {
            InitializeComponent();
        }

        private uint _red;
        private uint _black;
        private uint _kings;

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

        public uint Kings
        {
            get
            {
                return _kings;
            }
            set
            {
                _kings = value;
            }
        }

        private void LoadValues_Load(object sender, EventArgs e)
        {
            txtRed.Text = Red.ToString();
            txtBlack.Text = Black.ToString();
            txtKings.Text = Kings.ToString();

            txtRed.Focus();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Red = Convert.ToUInt32(txtRed.Text);
            Black = Convert.ToUInt32(txtBlack.Text);
            Kings = Convert.ToUInt32(txtKings.Text);

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
