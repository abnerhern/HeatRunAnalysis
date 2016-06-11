using System;
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

        private LoadingLimit pll;
        private LoadingLimit ltll;
        private LoadingLimit stll;
        
        // Loss of life 
        private LossOfLife lossNll;
        private LossOfLife lossPll;
        private LossOfLife lossLtll;
        private LossOfLife lossStll;

        private bool isUploaded = false;


        // Thresholds
        private double pllThesh = 120;
        private double ltllThresh = 140;
        private double stllThresh = 180;

        // Time interval
        private double timeInt = 1;

        //Ambient Temp for Zones
        private double ambientTemp = 30;

        // Transformer Life
        private double xfrmrLife = 180000;


        // Intialized Class
        public Form1()
        {
            InitializeComponent();

            subnameBox.Text = "Kramer";
            mvaBox.Text ="187";
            ambientBox.Text = "30";
            deltaThetaHS_RBox.Text = "28.6"; 
            deltaThetaTORBox.Text = "36";
            ratioBox.Text = "4.87";
            tauBox.Text = "3.5";

            coilWeightBox.Text = "123";
            tankWeightBox.Text = "123";
            oilVolumeBox.Text = "123";
            totalLossBox.Text = "123";

           // label14.

           // this.ControlBox = false;
            
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

            // Must check to see if there is any text on the field
            // Makes a temp

            try
            { 
                if (!this.isUploaded)
                {
                    throw new System.ArgumentException("No load profile uploaded", "original"); 
                }

                //ProgressForm pf = new ProgressForm();
                //pf.ShowDialog();

                // Set the transformer parameters
                setXfrmr();
                //pf.increaseProgress(20);
                

                // Set the loading limits and its formulas
                setLoadingLimits();
               // pf.increaseProgress(40);

                // Calculate loss of life
                calculateLoss();
               // pf.increaseProgress(50);

                

                // Paint the graphs
                paintGraph();
              //  pf.increaseProgress(60);

                //Populates the table
                populateTable();
              //  pf.increaseProgress(100);
                
                // Show the tables
                loadCover1.Visible = false;
                loadCover2.Visible = false;
                loadCover3.Visible = false;
                loadCover4.Visible = false;

                label14.Text = "Loss Of Life for Normal: " ;
                label16.Text = "Loss Of Life for PLL: " + pll.getLossOfLife() + "%";
                label17.Text = "Loss Of Life for LTELL: " + ltll.getLossOfLife() + "%";
                label18.Text = "Loss Of Life for STELL: " + stll.getLossOfLife() + "%";
              //  pf.Close();
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
            
            // Check to see if TauBox is empty
            if (!string.IsNullOrWhiteSpace(tauBox.Text))
            {
            this.xfrmr.setTauTO_R(Convert.ToDouble(tauBox.Text));
            }
            else
            {
                this.xfrmr.fillMoreInfoTwo(Convert.ToDouble(coilWeightBox.Text) ,
                Convert.ToDouble(tankWeightBox.Text),Convert.ToDouble(oilVolumeBox.Text),Convert.ToDouble(totalLossBox.Text) );
            }
 
          this.xfrmr.setTotalLife(this.xfrmrLife);

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
            this.nll = new NormalLoadingLimit(this.loadMult.getNormalLoadProfile(), this.xfrmr, 0, this.timeInt);
            this.pll = new LoadingLimit(this.loadMult.getPLLLoadProfile(), this.xfrmr, this.pllThesh, this.timeInt);
            this.ltll = new LoadingLimit(this.loadMult.getLTELLLoadProfile(), this.xfrmr, this.ltllThresh, this.timeInt);
            this.stll = new LoadingLimit(this.loadMult.getSTELLoadProfile(), this.xfrmr, this.stllThresh, this.timeInt);
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



            //============================= Stell Chart ====================================
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
                dataGridView1.Rows[i].Cells[6].Value = nll.getTau()[i]; // Tau
                dataGridView1.Rows[i].Cells[7].Value = lossNll.getFaa()[i]; // Faa
                dataGridView1.Rows[i].Cells[8].Value = lossNll.getFaa()[i]; // Aging Hour    
                dataGridView1.Rows[i].Cells[9].Value = lossNll.getCumAging()[i]; // Cummulative Aging  
            }

            //============================= PLL  =======================================

            // Data Grid View : 

            for (int i = 0; i < pll.getHottestSpotTemp().Length; i++)
            {
                dataGridView2.Rows.Add();
                dataGridView2.Rows[i].Cells[0].Value = i + 1; // Hour
                dataGridView2.Rows[i].Cells[1].Value = loadMult.getPLLLoadProfile()[i]; // Load
                dataGridView2.Rows[i].Cells[2].Value = 30; // Ambient Temp
                dataGridView2.Rows[i].Cells[3].Value = pll.getTopOilTemp()[i]; // Top Oil Temp
                dataGridView2.Rows[i].Cells[4].Value = pll.getHotSpotTemp()[i]; // Hot Spot Temp
                dataGridView2.Rows[i].Cells[5].Value = pll.getHottestSpotTemp()[i]; // Hottest Spot Temp 
                dataGridView2.Rows[i].Cells[6].Value = pll.getTau()[i]; // Tau
                dataGridView2.Rows[i].Cells[7].Value = lossPll.getFaa()[i]; // Faa
                dataGridView2.Rows[i].Cells[8].Value = lossPll.getFaa()[i]; // Aging Hour    
                dataGridView2.Rows[i].Cells[9].Value = lossPll.getCumAging()[i]; // Cummulative Aging  
            }

            //============================= LTELL  =======================================

            // Data Grid View : 

            for (int i = 0; i < ltll.getHottestSpotTemp().Length; i++)
            {
                dataGridView5.Rows.Add();
                dataGridView5.Rows[i].Cells[0].Value = i + 1; // Hour
                dataGridView5.Rows[i].Cells[1].Value = loadMult.getLTELLLoadProfile()[i]; // Load
                dataGridView5.Rows[i].Cells[2].Value = 30; // Ambient Temp
                dataGridView5.Rows[i].Cells[3].Value = ltll.getTopOilTemp()[i]; // Top Oil Temp
                dataGridView5.Rows[i].Cells[4].Value = ltll.getHotSpotTemp()[i]; // Hot Spot Temp
                dataGridView5.Rows[i].Cells[5].Value = ltll.getHottestSpotTemp()[i]; // Hottest Spot Temp 
                dataGridView5.Rows[i].Cells[6].Value = ltll.getTau()[i]; // Tau
                dataGridView5.Rows[i].Cells[7].Value = lossLtll.getFaa()[i]; // Faa
                dataGridView5.Rows[i].Cells[8].Value = lossLtll.getFaa()[i]; // Aging Hour    
                dataGridView5.Rows[i].Cells[9].Value = lossLtll.getCumAging()[i]; // Cummulative Aging  
            }

            //============================= STELL  =======================================

            // Data Grid View : 

            for (int i = 0; i < stll.getHottestSpotTemp().Length; i++)
            {
                dataGridView4.Rows.Add();
                dataGridView4.Rows[i].Cells[0].Value = i + 1; // Hour
                dataGridView4.Rows[i].Cells[1].Value = loadMult.getSTELLoadProfile()[i]; // Load
                dataGridView4.Rows[i].Cells[2].Value = 30; // Ambient Temp
                dataGridView4.Rows[i].Cells[3].Value = stll.getTopOilTemp()[i]; // Top Oil Temp
                dataGridView4.Rows[i].Cells[4].Value = stll.getHotSpotTemp()[i]; // Hot Spot Temp
                dataGridView4.Rows[i].Cells[5].Value = stll.getHottestSpotTemp()[i]; // Hottest Spot Temp 
                dataGridView4.Rows[i].Cells[6].Value = stll.getTau()[i]; // Tau
                dataGridView4.Rows[i].Cells[7].Value = lossStll.getFaa()[i]; // Faa
                dataGridView4.Rows[i].Cells[8].Value = lossStll.getFaa()[i]; // Aging Hour    
                dataGridView4.Rows[i].Cells[9].Value = lossStll.getCumAging()[i]; // Cummulative Aging  
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

        private void tempLimits_Click(object sender, EventArgs e)
        {
            // Open form that changes the temperature limitations

            ThresholdForm tf = new ThresholdForm(pllThesh.ToString() ,ltllThresh.ToString(), stllThresh.ToString() );

            tf.ShowDialog();


            
                this.pllThesh = tf.getPllThresh();
                this.ltllThresh = tf.getLtllThresh();
                this.stllThresh = tf.getSTLLThresh();

                tf.Close();

           
           
        }

        //
        private void hourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hourToolStripMenuItem.Checked = true;
            minToolStripMenuItem.Checked = false;
            dayToolStripMenuItem.Checked = false;
            this.timeInt = 1;
        }

        private void minToolStripMenuItem_Click(object sender, EventArgs e)
        {
            minToolStripMenuItem.Checked = true;
            hourToolStripMenuItem.Checked = false;
            dayToolStripMenuItem.Checked = false;
            this.timeInt = 0.5;
        }

        private void dayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dayToolStripMenuItem.Checked = true;
            hourToolStripMenuItem.Checked = false;
            minToolStripMenuItem.Checked = false;
            this.timeInt = 24;
        }

        // Temperature Zone Buttons
        private void zone1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zone1ToolStripMenuItem.Checked = true;
            zone2ToolStripMenuItem.Checked = false;
            zone3ToolStripMenuItem.Checked = false;
            zone4ToolStripMenuItem.Checked = false;


        }

        private void zone2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zone1ToolStripMenuItem.Checked = false;
            zone2ToolStripMenuItem.Checked = true;
            zone3ToolStripMenuItem.Checked = false;
            zone4ToolStripMenuItem.Checked = false;
        }

        private void zone3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zone1ToolStripMenuItem.Checked = false;
            zone2ToolStripMenuItem.Checked = false;
            zone3ToolStripMenuItem.Checked = true;
            zone4ToolStripMenuItem.Checked = false;
        }

        private void zone4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zone1ToolStripMenuItem.Checked = false;
            zone2ToolStripMenuItem.Checked = false;
            zone3ToolStripMenuItem.Checked = false;
            zone4ToolStripMenuItem.Checked = true;
        }

        // Open GUI for Transformer Life
        private void setTransformerLifeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransfomerLifeForm tl = new TransfomerLifeForm(xfrmrLife.ToString());

            tl.ShowDialog();

            this.xfrmrLife = tl.getXfrmrLife();

            tl.Close();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    
    
    }
}
