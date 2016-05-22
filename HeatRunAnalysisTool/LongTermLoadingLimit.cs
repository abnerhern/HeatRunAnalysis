using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * *************************************
 * FileRetrieval.cs
 * Modified by Abner Joel Hernandez
 * Created by Aaron Bautista
 * Date: 2/13/2016
 * Version 0.1
 * 
 * ************************************
 */


namespace HeatRunAnalysisTool
{
    class LongTermLoadingLimit
    {

//********************************************************MEMBER VARIABLES************************************************************
        //This will store our perunit values
        private double[] perUnitValues;

        private double kRMS;

        // This will store our load hours
        // Since we know the hours and it is going
        //to be hours 0-3, we can define it
        

        // This will store our hotspot temp
        private double[] ultimateTopOil; // Ultimate top oil rise. Used for top oil and tauO calculations
        private double[] hotSpotTemp;
        private double[] topOilTemp;
        private double[] hottestSpotTemp;
        SubstationTransformer xfrmr;

        private double[] tauTO; // TauO

        private int t; // Time interval. 1 is hour. 0.5 is hald an hour

        public LongTermLoadingLimit() { }

        public LongTermLoadingLimit(double[] perUnitValues, SubstationTransformer xfrmr) 
        {
            this.perUnitValues = perUnitValues;
            this.xfrmr = xfrmr;

            calculateKRMS();

            ultimateTopOil = new double[perUnitValues.Length];
            hotSpotTemp = new double[perUnitValues.Length];
            topOilTemp = new double[perUnitValues.Length];
            hottestSpotTemp = new double[perUnitValues.Length];

            // Check to see if we need to calculate tauTO or if n = 1 the tauTO = tauTR
            calculateTauTO();

            calculateUltimateTopOil();

            calculateTopOilTemp();
            calculateHotSpotTemp();
            calculateHottestSpotTemp();
        
        }


//**************************************************************METHODS***************************************************************
        
        private void calculateUltimateTopOil()
        {
            double topOilUltimate = 0;

            for (int i = 0; i < topOilTemp.Length; i++)
            {
                if (i == 0)
                {
                    // We must calculate top oil temperature using KRMS;
                    // Define Top Oil Ultimate
                    topOilUltimate = (xfrmr.getdeltaThetaTO_R()) *
                    Math.Pow(((kRMS * kRMS * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());

                    // Console.WriteLine("This should be 26.8 : " +topOilUltimate);
                    ultimateTopOil[i] = Math.Round(topOilUltimate, 2);
                    continue;
                }
                // Calculate top Oil Ultimate first then calculate TOP oil
                topOilUltimate = (xfrmr.getdeltaThetaTO_R()) * Math.Pow(((perUnitValues[i - 1] * perUnitValues[i - 1]
                                  * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());

                ultimateTopOil[i] = Math.Round(topOilUltimate, 2);
            }

        }
        
        
        
        private void calculateTopOilTemp()
        {

            for (int i = 0; i < topOilTemp.Length; i++)
            {
                if(i == 0)
                {
                    topOilTemp[0]  = ultimateTopOil[0]; 
                    continue;
                }

                topOilTemp[i] = Math.Round( (ultimateTopOil[i] * (1 - Math.Exp(-1 / tauTO[i])))
                                               + (topOilTemp[i-1] * Math.Exp(-1/tauTO[i]) ) , 2);                  

             }
        
        }

        private void calculateHotSpotTemp()
        {
            for (int i = 0; i < perUnitValues.Length; i++)
            {   
                    if (hottestSpotTemp[i] > 140)
                    {
                        hotSpotTemp[i] = Math.Round(xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i -1], 2 * xfrmr.getM()), 2);
                    }

                    else if (hottestSpotTemp[i] < 140)
                    {
                        hotSpotTemp[i] = Math.Round(xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i], 2 * xfrmr.getM()), 2);
                    }       
            }
        
        }

        private void calculateHottestSpotTemp()
        {

            for (int i = 0; i < hottestSpotTemp.Length; i++)
            {
                hottestSpotTemp[i] = Math.Round(topOilTemp[i] + hotSpotTemp[i] + xfrmr.getAmbientTemp(), 2);
            }
        }


        private void calculateKRMS()
        {
            double krmsstore = Math.Sqrt((Math.Pow(perUnitValues[perUnitValues.Length - 1], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 2], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 3], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 4], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 5], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 6], 2)) / 6);

            // To change later
            this.kRMS = Math.Round(krmsstore, 3);
        }


        private void calculateTauTO()
        {
            if (xfrmr.getN() == 1)
            {
                tauTO = new double[hotSpotTemp.Length];
                for (int i = 0; i < tauTO.Length; i++)
                {

                    tauTO[i] = xfrmr.getTauTO_R();
                }
            }
            else
            {
                // TODO
                tauTO = new double[hotSpotTemp.Length];
                for (int i = 0; i < tauTO.Length; i++)
                {
                    if (i == tauTO.Length - 1)
                    {
                        tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[0] / xfrmr.getTauTO_R()) - (ultimateTopOil[i] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[0] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));
                        continue;
                    }

                    tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[i + 1] / xfrmr.getTauTO_R()) - (ultimateTopOil[i] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[i + 1] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));
                }


            }
        }



        // Prints the info
        public void printInfo()
        {
            Console.WriteLine();
            Console.WriteLine("LTELL (Load Cycles and Temperature Rises for " + xfrmr.getSubstationName() );
            Console.WriteLine();

            for (int i = 0; i < hotSpotTemp.Length; i++)
            {
                Console.WriteLine("LOAD HOUR: " + (i+1) + "\tLOAD PU: "
                    + perUnitValues[i] + "\tTOP OIL TEMP: " + topOilTemp[i] + "\tHOT SPOT TEMP: " + hotSpotTemp[i] + "\tHOTTEST SPOT TEMP: " + hottestSpotTemp[i] + "\t Tau " + tauTO[i]);
            }
        }

//**********************************************************GETTERS******************************************************************

        public double[] getHottestSpotTemp()
        {
            return this.hottestSpotTemp;
        }


    }
}
