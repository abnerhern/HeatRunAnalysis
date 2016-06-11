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
    public partial class ThresholdForm : Form
    {
        private double pllThesh;
        private double ltllThresh;
        private double stllThresh;

        public ThresholdForm(String pll, String ltll, String stll)
        {
            InitializeComponent();
            textBox1.Text = pll;
            textBox2.Text = ltll;
            textBox3.Text = stll;

            this.pllThesh = Convert.ToDouble(textBox1.Text);
            this.ltllThresh = Convert.ToDouble(textBox2.Text);
            this.stllThresh = Convert.ToDouble(textBox3.Text);

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        // What happens when this loads.
        private void ThresholdForm_Load(object sender, EventArgs e)
        {

        }

        // When Clicked Okay:
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.pllThesh = Convert.ToDouble(textBox1.Text);
                this.ltllThresh = Convert.ToDouble(textBox2.Text);
                this.stllThresh = Convert.ToDouble(textBox3.Text);

                this.Close();
                
            }
            catch 
            {
                MessageBox.Show("Make sure the input is valid.", "Error",
               MessageBoxButtons.OK, MessageBoxIcon.Error);    
            }


        }
        
        // When clicked Cancel
        private void button2_Click(object sender, EventArgs e)
        {
           
            this.Close();
        }

        //==============================Getters==================================

        public double getPllThresh()
        {
            return this.pllThesh;
        }

        public double getLtllThresh() 
        {
            return this.ltllThresh;
        }

        public double getSTLLThresh()
        {
            return this.stllThresh;
        }

       
    }
}
