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
    public partial class TransfomerLifeForm : Form
    {

        private double xfrmrLife;

        public TransfomerLifeForm(String xfrmrLife)
        {
            InitializeComponent();

            this.xfrmrLife = Convert.ToDouble(xfrmrLife);
            textBox1.Text = xfrmrLife;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                xfrmrLife = Convert.ToDouble(textBox1.Text);
                this.Close();
            }
            catch 
            {
                MessageBox.Show("Make sure the input is valid.", "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public double getXfrmrLife() 
        {
            return this.xfrmrLife;
        }

    }
}
