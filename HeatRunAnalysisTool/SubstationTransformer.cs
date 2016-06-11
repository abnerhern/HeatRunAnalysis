using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * *************************************
 * FileRetrieval.cs
 * Created by Abner Joel Hernandez
 * Date: 2/13/2016
 * Version 0.1
 * 
 * ************************************
 */

namespace HeatRunAnalysisTool
{
    class SubstationTransformer
    {
        

//**********************************************MEMBER VARIABLES*******************************************************************

        private string substation_name; // Name of the Transformer
        private double mvaRating;
        private double ambientTemp; //Ambient Temp Might be an array if this value isn’t constant

        /*ONAN = 1, ONAF = 2, Non-Directed OFAF or OFWF = 3,
         Directed ODAF or ODWF = 4*/
        private int cooling_mode = 0; // Stores the integer value to represent ONAN/ONAF etc.
       
        private double deltaThetaHS_R; // Hottest Spot conductor rise over topoil temp
        private double deltaThetaTO_R; // Top oil rise over ambient temperature at rated load
        private double R; // Ratio at load loss at rated load to no load loss.


        
               
        private double m; // Determined by the cooling factor
        private double n; // Determined by the cooling factor


        private double pT_R; // Total loss in watts at rated load
        private double corecoil_weight;
        private double tank_weight;
        private double oil_volume;
        private double C = 0; // Initialize value for C 
        private double tauTO_R; // Time constant at rated kilo volt ampere, we get this when 
        // if n = 1 then, tauO = tauR. To be calculated later in a PLL class or equivalent

        private double totalLife = 180000;

        
//****************************************************CONSTRUCTOR*******************************************************************
//*****************************Takes in the parameters of substation name, mvarating, loadprofile***********************************

        // Constructor that takes no parameters
        public SubstationTransformer() { }

        public SubstationTransformer(string substation_name, double mvaRating,  double ambientTemp)
        {
            this.substation_name = substation_name;
            this.mvaRating = mvaRating;
            this.ambientTemp = ambientTemp;
        }

        // To finish making a substation class we need to call this function.
        public void fillMoreInfo(int cooling_mode, double deltaThetaHS_R, double deltaThetaTO_R, double R)
        {
            this.cooling_mode = cooling_mode;
            this.deltaThetaHS_R = deltaThetaHS_R;
            this.deltaThetaTO_R = deltaThetaTO_R;
            this.R = R;
            // Finds value of M and M
            coolmodeToMN(); 
        }

        // To fully finish making the substation class, call on this function
        public void fillMoreInfoTwo(double corecoil_weight, double tank_weight, double oil_volume, double pT_R)
        {
            this.corecoil_weight = corecoil_weight;
            this.tank_weight = tank_weight;
            this.oil_volume = oil_volume;
            this.pT_R = pT_R;
            
        }

//**************************************************************METHODS***************************************************************


        // Finds m and n depending on the cooling mode
        private void coolmodeToMN() 
        {
            /*ONAN = 1, ONAF = 2, Non-Directed OFAF or OFWF = 3,
            Directed ODAF or ODWF = 4*/
            if(cooling_mode == 1)
            {
                this.m = 0.8;
                this.n = 0.8;
                return;
            }
            else if(cooling_mode == 2)
            {
                this.m = 0.8;
                this.n = 0.9;
                return;
            }
            else if (cooling_mode == 3)
            {
                this.m = 0.8;
                this.n = 0.9;
                return;
            }
            else if(cooling_mode == 4)
            {
                this.m = 1.0;
                this.n = 1.0;
                return;
            }
        }

        // Get K from load factor
      

        /* This will use oil_gallons , tank_weight, 
         * corecoil_weight and cooling mode 
         * to find the value of C. Then store it into C.
         *  1 is for weight in kilo and volume in liters*/
        public void calculateC(int kiloOrPounds)
        {
            /*ONAN = 1, ONAF = 2, Non-Directed OFAF 
             * or OFWF = 3,
             * Directed ODAF or ODWF = 4*/

            // For ONAN and ONAF. When oil is natural
            if(cooling_mode == 1 || cooling_mode == 2) 
            {
                switch (kiloOrPounds) 
                {
                        // In kilo and liters
                    case 1:
                        C = (0.1323 * corecoil_weight) + (0.0882 * tank_weight) + (0.3513 * oil_volume);
                        break;
                        // In pounds and gallons
                    case 2:
                        C = (0.06 * corecoil_weight) + (0.04 * tank_weight) + (1.33 * oil_volume);
                        break;
                }
                
                
            }
            else
            {
                switch (kiloOrPounds)
                {
                    // In kilo and liters
                    case 1:
                        C = (0.1323 * corecoil_weight) + (0.1323 * tank_weight) + (0.5099 * oil_volume);
                        break;
                    // In pounds and gallons
                    case 2:
                        C = (0.06 * corecoil_weight) + (0.06 * tank_weight) + (1.93 * oil_volume);
                        break;
                }
            }

        }


        private void calculateTauTO_R()
        { 
            this.tauTO_R = (this.C * this.deltaThetaTO_R) / this.pT_R; 
        }
        
     

//***************************************************GETTERS********************************************************
        // Getters
        public string getSubstationName() 
        {
            return substation_name;
        }

        public double getMVARating()
        {
            return mvaRating;
        }

        public double getAmbientTemp()
        {
            return ambientTemp;
        }

        public int getCoolingMode()
        {
            return cooling_mode;
        }

        public double getdeltaThetaHS_R()
        {
            return this.deltaThetaHS_R;
        }

        public double getdeltaThetaTO_R()
        {
            return this.deltaThetaTO_R;
        }

        public double getR()
        {
            return this.R;
        }

        public double getM() 
        {
            return this.m;
        }

        public double getN()
        {
            return this.n;
        }

        public double getTauTO_R() 
        {
            return this.tauTO_R;
        }

        public double getTotalLife()
        {
            return this.totalLife;
        }


//***************************************************SETTERS********************************************************
        
        public void setSubstationName(string substation_name)
        {
            this.substation_name = substation_name;
        }

        public void setMVARating(double mvaRating)
        {
            this.mvaRating = mvaRating;
        }

        // For testing purposes.
        public void setTauTO_R(double tauTO_R)
        {
            this.tauTO_R = tauTO_R;
        }

        public void setTotalLife(double totalLife)
        {
            this.totalLife = totalLife;
        }

    }
}
