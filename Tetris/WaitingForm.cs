using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris.Model;

namespace Tetris
{
    partial class WaitingForm : Form
    {
        public bool playing;
        public WaitingForm(TetrisGrid grid)
        {
            InitializeComponent();
             playing = false;          
            this.ShowDialog();
                  
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {         
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            playing = true;
            this.Close();
        }
    }
}
