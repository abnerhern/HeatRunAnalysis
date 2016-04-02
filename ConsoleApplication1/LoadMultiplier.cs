using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * *************************************
 * FileRetrieval.cs
 * Created by Abner Joel Hernandez
 * Date: 2/18/2016
 * Version 0.1
 * 
 * ************************************
 */


namespace HeatRunAnalysis
{
    class LoadMultiplier
    {

//**********************************************MEMBER VARIABLES*******************************************************************
        //From normal load
        private double[] loadMultSCE = new double[3] { 1.1, 1.4, 1.6 };

        private double[] loadMultIEEE = new double[3] { 1.26, 1.39, 2.06 };

        private double[] normalLP;
        private double[] pllLP;
        private double[] ltellLP;
        private double[] stellLP;

        private int maxIndex;


//****************************************************CONSTRUCTOR*******************************************************************
//*****************************Takes in the parameters of  loadProfile and loadMultiplierSelection *********************************

        // Constructor with no parameters
        public LoadMultiplier() { }

        public LoadMultiplier(double[] normalLP,int loadMultSelection ) 
        {
            
            findMaxIndex(normalLP);
            this.normalLP = normalLP;

            

            switch(loadMultSelection)
            {
                case 1:
                    calculateSCELoadProfile(normalLP);
                    break;
                case 2:
                    calculateIEEELoadProfile(normalLP);
                    break;
            }
        }
//**********************************************************METHODS*******************************************************************
//******************************************These function calculates the load multiplier*********************************************

        // Calculate Load Profile for SCE loading limits 
        private void calculateSCELoadProfile(double[] loadProfile)
        {
            double[] temp = new double[loadProfile.Length];
            
            // For Pll
            for (int i = 0; i < loadProfile.Length; i++ )
            {
                temp[i] = loadProfile[i] * loadMultSCE[1];

            }
            this.pllLP = temp;
            

            
        }

                
        // Calculate Load Profile for IEEE loading limit
        private void calculateIEEELoadProfile(double[] loadProfile)
        {
            // Temporary array to store data
            this.pllLP = new double[loadProfile.Length];

            // For Pll
            for (int i = 0; i < loadProfile.Length; i++)
            {
                this.pllLP[i] = Math.Round(loadProfile[i] * this.loadMultIEEE[0], 2);
            }
            

            //LTELL Loading profile is the same as PLL, but with some adjustments
            this.ltellLP = new double[loadProfile.Length];

            // Finds the LTELL loading profile
            for (int i = 0; i < ltellLP.Length; i++)
            {
                
                if( i >= maxIndex - 5 && i <= maxIndex )
                {
                    this.ltellLP[i] = Math.Round(loadProfile[i] * loadMultIEEE[1], 2);
                    continue;
                }
                
                this.ltellLP[i] = Math.Round(loadProfile[i] * loadMultIEEE[0], 2);

            }
                
            //STELL Loading profile is the same as LTELL, but with some adjustments
            // Finds the LTELL loading profile
            this.stellLP = new double[loadProfile.Length];
            for (int i = 0; i < stellLP.Length; i++)
            {

                if (i >= maxIndex - 5 && i <= maxIndex)
                {
                    this.stellLP[i] = Math.Round(loadProfile[i] * loadMultIEEE[1], 2);
                    continue;
                }

                this.stellLP[i] = Math.Round(loadProfile[i] * loadMultIEEE[0], 2);
            }

            
            this.stellLP[maxIndex - 5] = Math.Round(loadProfile[maxIndex - 5] * loadMultIEEE[2], 2);
                
            
        
        }

        // Finds the index of the maximum value of the load profile
        private void findMaxIndex(double[] loadProfile)
        {
            int currindex = 0;
            // Stores current max value;
            double currMax = loadProfile[0];

            for (int i = 0; i < loadProfile.Length; i++ )
            {
                if(currMax <= loadProfile[i])
                {
                    currMax = loadProfile[i];
                    currindex = i; 
                
                }

            }
            this.maxIndex = currindex;
        
        }


//***************************************************GETTERS********************************************************
        
        public double[] getNormalLoadProfile() 
        {
            return this.normalLP;
        }        
        
        public double[] getPLLLoadProfile()
        {
        return this.pllLP;
        }

        public double[] getLTELLLoadProfile()
        {
            return this.ltellLP;
        }

        public double[] getSTELLoadProfile()
        {
            return this.stellLP;
        }

        

    }
}
