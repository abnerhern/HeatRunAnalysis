﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


// Written by Abner Hernandez


namespace HeatRunAnalysisTool
{
    // Subclass
    public partial class Form1 : Form
    {
        // For the CSV file
        private CSVFileRetrieval csvR;

        // Transformer Parameters
        private SubstationTransformer xfrmr;
        private LoadMultiplier loadMult;

        // Loading Limits
        private NormalLoadingLimit nll;
        private PlannedLoadingLimit pll;
        private LongTermLoadingLimit ltll;
        private ShortTermLoadingLimit stll;
        
        // Loss of life 
        private LossOfLife lossNll;
        private LossOfLife lossPll;
        private LossOfLife lossLtll;
        private LossOfLife lossStll;

        private bool isUploaded = false;


        // Intialized Class
        public Form1()
        {
            InitializeComponent();
            
        }

        // To make the windows draggable
        protected override void WndProc(ref Message m)
        {
            // This code allows the window to be dragged around
            switch(m.Msg)
            { 
                // Check case of address/reference
                case 0x84:
                base.WndProc(ref m);
                if ((int)m.Result == 0x1)
                    m.Result = (IntPtr)0x2; // Get pointer to Address
                return;


            }

            base.WndProc(ref m);
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // For the X button to escape
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();  
        }

        // Entire Panel
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        // Run All for heat run analysis
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // MessageBox.Show(mvaBox.Text);

            // Must check to see if there is any text on the field
            // Makes a temp

            try
            {
                if (!this.isUploaded)
                {
                    throw new System.ArgumentException("No load profile uploaded", "original"); 
                }

                // Set the transformer parameters
                setXfrmr();
                

                // Set the loading limits and its formulas
                setLoadingLimits();

                // Calculate loss of life
                calculateLoss();

                

                // Paint the graphs
                paintGraph();

                //Populates the table
                populateTable();
                
                // Show the tables
                loadCover1.Visible = false;
                loadCover2.Visible = false;
                loadCover3.Visible = false;
                loadCover4.Visible = false;
            }
           catch(Exception e1)
            {
                MessageBox.Show("Unable to run analysis. Make sure the values are correct and load profile uploaded.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        private void setXfrmr()
        {
            this.xfrmr = new SubstationTransformer(subnameBox.Text, Convert.ToDouble(mvaBox.Text), Convert.ToDouble(ambientBox.Text));

            int coolingMode = -1;
            coolingMode = getCoolValue();

            // fill Susbtation transformer parameters
            this.xfrmr.fillMoreInfo(coolingMode, Convert.ToDouble(deltaThetaHS_RBox.Text), Convert.ToDouble(deltaThetaTORBox.Text), Convert.ToDouble(ratioBox.Text));
            this.xfrmr.setTauTO_R(Convert.ToDouble(tauBox.Text));
        }


        private int getCoolValue() 
        {
            if(OnanButton.Checked){return 1;}
            else if(ONAFButton.Checked){return 2;}
            else if(nonDirectedOFAF.Checked){return 3;}
            else if(DirectedOFAFButton.Checked){return 4;}

            return -1;
        }

        private void setLoadingLimits() 
        {
            this.nll = new NormalLoadingLimit(this.loadMult.getNormalLoadProfile(), this.xfrmr);
            this.pll = new PlannedLoadingLimit(this.loadMult.getPLLLoadProfile(), this.xfrmr);
            this.ltll = new LongTermLoadingLimit(this.loadMult.getLTELLLoadProfile(), this.xfrmr);
            this.stll = new ShortTermLoadingLimit(this.loadMult.getSTELLoadProfile(), this.xfrmr);
        }

        private void calculateLoss()
        { 
            lossNll = new LossOfLife(this.loadMult.getNormalLoadProfile(), this.nll.getHottestSpotTemp());
            lossPll = new LossOfLife(this.loadMult.getPLLLoadProfile(), this.pll.getHottestSpotTemp());
            lossLtll = new LossOfLife(this.loadMult.getLTELLLoadProfile(), this.ltll.getHottestSpotTemp());
            lossStll = new LossOfLife(this.loadMult.getSTELLoadProfile(), this.stll.getHottestSpotTemp());
        }

        private void paintGraph()
        {
            //============================= Normal Chart ====================================
            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(0, 140); 
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, nll.getHottestSpotTemp().Length + 1); 
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true; 
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;


            for (int i = 0; i < nll.getHottestSpotTemp().Length; i++ )
            {

                chart1.Series["normalseries"].Points.AddXY(i + 1, nll.getHottestSpotTemp()[i]);
            }

            chart1.Series["normalseries"].Color = Color.Blue; 



            //============================= Pll Chart ====================================

            chart2.ChartAreas[0].AxisY.ScaleView.Zoom(0, 140);
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(0, pll.getHottestSpotTemp().Length + 1);
            chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

           

            for (int i = 0; i < pll.getHottestSpotTemp().Length; i++)
            {

                chart2.Series["Series1"].Points.AddXY(i + 1, pll.getHottestSpotTemp()[i]);
                
            }
                chart2.Series["Series1"].Color = Color.Green;  



            //============================= Ltell Chart ====================================

            chart3.ChartAreas[0].AxisY.ScaleView.Zoom(0, 140);
            chart3.ChartAreas[0].AxisX.ScaleView.Zoom(0, ltll.getHottestSpotTemp().Length + 1);
            chart3.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].AxisX.ScaleView.Zoomable = true;


            for (int i = 0; i < ltll.getHottestSpotTemp().Length; i++)
            {

                chart3.Series["Series1"].Points.AddXY(i + 1, ltll.getHottestSpotTemp()[i]);
                
            }
                chart3.Series["Series1"].Color = Color.Yellow; 



            //============================= stell Chart ====================================
            chart4.ChartAreas[0].AxisY.ScaleView.Zoom(0, 140);
            chart4.ChartAreas[0].AxisX.ScaleView.Zoom(0, stll.getHottestSpotTemp().Length + 1);
            chart4.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart4.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart4.ChartAreas[0].AxisX.ScaleView.Zoomable = true;

           

            for (int i = 0; i < stll.getHottestSpotTemp().Length; i++)
            {

                chart4.Series["Series1"].Points.AddXY(i + 1, stll.getHottestSpotTemp()[i]);

            }
                chart4.Series["Series1"].Color = Color.Red; 



            //============================= ALL Chart ======================================
            chart5.ChartAreas[0].AxisY.ScaleView.Zoom(0, 140);
            chart5.ChartAreas[0].AxisX.ScaleView.Zoom(0, stll.getHottestSpotTemp().Length + 1);
            chart5.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart5.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart5.ChartAreas[0].AxisX.ScaleView.Zoomable = true;



            for (int i = 0; i < nll.getHottestSpotTemp().Length; i++)
            {
                chart5.Series["Normal"].Points.AddXY(i + 1, nll.getHottestSpotTemp()[i]);
                chart5.Series["PLL"].Points.AddXY(i + 1, pll.getHottestSpotTemp()[i]);
                chart5.Series["LTELL"].Points.AddXY(i + 1, ltll.getHottestSpotTemp()[i]);
                chart5.Series["STELL"].Points.AddXY(i + 1, stll.getHottestSpotTemp()[i]);       
            }

                  chart5.Series["Normal"].Color = Color.Blue;
                  chart5.Series["PLL"].Color = Color.Green;
                  chart5.Series["LTELL"].Color = Color.Yellow;
                  chart5.Series["STELL"].Color = Color.Red;
        
        }

        private void populateTable()
        {
            // **************************** Tables ****************************************


            //============================= Normal  =======================================

            // Data Grid View : 

            for (int i = 0; i < nll.getHottestSpotTemp().Length; i++)
            {   
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = i + 1; // Hour
                dataGridView1.Rows[i].Cells[1].Value = loadMult.getNormalLoadProfile()[i]; // Load
                dataGridView1.Rows[i].Cells[2].Value = 30; // Ambient Temp
                dataGridView1.Rows[i].Cells[3].Value = nll.getTopOilTemp()[i]; // Top Oil Temp
                dataGridView1.Rows[i].Cells[4].Value = nll.getHotSpotTemp()[i]; // Hot Spot Temp
                dataGridView1.Rows[i].Cells[5].Value = nll.getHottestSpotTemp()[i]; // Hottest Spot Temp 
                dataGridView1.Rows[i].Cells[6].Value = 3; // Tau
                dataGridView1.Rows[i].Cells[7].Value = lossNll.getFaa()[i]; // Faa
                dataGridView1.Rows[i].Cells[8].Value = lossNll.getFaa()[i]; // Aging Hour    
                dataGridView1.Rows[i].Cells[9].Value = lossNll.getCumAging()[i]; // Cummulative Aging  
            }

        }



        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        // Minimize button
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // If they click to open something
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Comma Separated Values File(.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            // If they click OK
           if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
           {
               // Get the string path
               string filepath = openFileDialog1.FileName;
               // Start File Retrieval

               try
               {
                   CSVFileRetrieval csvR = new CSVFileRetrieval(filepath);
                   this.csvR = csvR;
                   this.isUploaded = true;
                   // Puts the load multiplier in the array
                   loadMult = new LoadMultiplier(csvR.getArray(), 2);

                   // Display that it has been successfully scanned
                   MessageBox.Show("File Sucessfully Loaded", "File Retrieval", MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
               catch(Exception e1)
               {
                   MessageBox.Show("Could not load the file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }

           }

        }

        // Drop Down for exiting
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        // Grid View of the Normal Data
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Grid View of the Planned Data
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //
        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        // Not needed
        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }
    
    
    }
}