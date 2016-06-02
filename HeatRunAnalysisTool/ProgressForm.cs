using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeatRunAnalysisTool
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();

            this.ControlBox = false;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
           // progressBar1.Step = 1;
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        public void increaseProgress(int point) 
        {          
                progressBar1.Value = point;
                progressBar1.Refresh();
                //label1.Text = progressBar1.Value.ToString();
        }


    }
}
