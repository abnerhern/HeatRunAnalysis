using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatRunAnalysisTool
{
    class LoadingLimit
    {
        
        //This will store our perunit values
        private double[] perUnitValues;

        // This will store our load hours
        // Since we know the hours and it is going
        //to be hours 0-3, we can define it
        

        private double kRMS;

        // This will store our hotspot temp
        private double[] ultimateTopOil; // Ultimate top oil rise. Used for top oil and tauO calculations
        private double[] hotSpotTemp;
        private double[] topOilTemp;
        private double[] hottestSpotTemp;

        private double[] tauTO; // TauO

        // Loss of Life Parameters
        private double[] Faa;
        private double[] Aging_hour;
        private double[] Cum_Aging_hour;
        private double[] LossOfLifeHour;

        private double LossOfLifeTotal;


        private double t; // Time interval. 1 is hour. 0.5 is half an hour

        private SubstationTransformer xfrmr; // Store transformer characteristics

        private double threshold; //Stores the threshold value for the maximum

        private int maxPerUnitIndex; // Stores the maximim perunit value possible for this loading limit
        private double maxPerUnit; // Stores the maximim perunit value possible for this loading limit
        
        public LoadingLimit() { }

        public LoadingLimit(double[] perUnitValues, SubstationTransformer xfrmr, double threshold, double t)
        {
            // Store the values of the array, transformer characerstics and the threshold values
            this.perUnitValues = perUnitValues;
            this.xfrmr = xfrmr;
            this.threshold = threshold;
            this.t = t;

            this.maxPerUnit = perUnitValues.Max();
            this.maxPerUnitIndex = Array.IndexOf(perUnitValues, maxPerUnit);
            
            calculateKRMS();

            // Sets the array lengths for each array
            ultimateTopOil = new double[perUnitValues.Length + 1];
            hotSpotTemp = new double[perUnitValues.Length];
            topOilTemp= new double[perUnitValues.Length];
            hottestSpotTemp = new double[perUnitValues.Length];
            tauTO = new double[perUnitValues.Length];
            Faa = new double[perUnitValues.Length];
            Aging_hour = new double[perUnitValues.Length];
            Cum_Aging_hour = new double[perUnitValues.Length];
            LossOfLifeHour = new double[perUnitValues.Length];


            // Calculate Information:
            calculateInfo();
        
        }

        //**************************************************************METHODS***************************************************************

        private void calculateInfo()
        {
            // We have Krms, so we can calculate the following for the FIRST iteration
            double TOInitial = 0;
            double TOUlt = 0;

            for (int i = 0; i < topOilTemp.Length; i++)
            {
                // If we are at the first iteration We must calculate the Tau at this iteration
                if (i == 0)
                {
                    // Initial TO before iterations
                    TOInitial = (xfrmr.getdeltaThetaTO_R()) *  Math.Pow(((kRMS * kRMS * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());
                    // Now that we have this, we can find the current iteration value
                    TOUlt = (xfrmr.getdeltaThetaTO_R()) * Math.Pow(((perUnitValues[i] * perUnitValues[i] * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());

                    // Calculate Tau
                    calculateTauTO(TOUlt, TOInitial, i);

                    // Use Top Oil Initial for Krms as First TO Value
                    topOilTemp[i] = TOInitial;

                    // Next find HotSpot Temperature
                    hotSpotTemp[i] = xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i], 2 * xfrmr.getM());

                    // Get Hottest Spot Temperautre
                    hottestSpotTemp[i] = topOilTemp[i] + hotSpotTemp[i] + xfrmr.getAmbientTemp();
                  
                    // Continue the rest of the loop
                    continue;
                }

                // Initial TO before iterations
                TOInitial = topOilTemp[i - 1];
                // Now that we have this, we can find the current iteration value
                TOUlt = (xfrmr.getdeltaThetaTO_R()) * Math.Pow(((perUnitValues[i] * perUnitValues[i] * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());

                // Calculate Tau
                calculateTauTO(TOUlt, TOInitial, i);

                // Use Top Oil Initial for Krms as First TO Value
                topOilTemp[i] = (TOUlt * (1 - Math.Exp(-t / tauTO[i]))) + (TOInitial * Math.Exp(-t / tauTO[i]));

                // Next find HotSpot Temperature
                hotSpotTemp[i] = xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i], 2 * xfrmr.getM());

                // Get Hottest Spot Temperautre
                hottestSpotTemp[i] = topOilTemp[i] + hotSpotTemp[i] + xfrmr.getAmbientTemp();

                // Check threshold
                if (hottestSpotTemp[i] > threshold)
                {
                    perUnitValues[i] = perUnitValues[i] - 0.01;
                    i--;
                }

                // Check if max is below threshold. If so increase it, until it is below threshold
                // Make sure to stop increasing it at a certain point or else infinite loop!

            }

            this.maxPerUnit = perUnitValues.Max();
        }


        private void calculateKRMS()
        {
            double krmsstore = Math.Sqrt((Math.Pow(perUnitValues[perUnitValues.Length - 1], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 2], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 3], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 4], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 5], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 6], 2)) / 6);

            // To change later
            this.kRMS = Math.Round(krmsstore, 3);
        }

        // Takes Ultimate, initial and index to put the Tau value
        private void calculateTauTO(double TOUlt ,double TOInit, int i)
        {
            tauTO[i] = xfrmr.getTauTO_R() * ((TOUlt / xfrmr.getTauTO_R()) - (TOInit / xfrmr.getTauTO_R())) / (Math.Pow(TOUlt / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(TOInit / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));
            
        }


        //**********************************************************GETTERS******************************************************************

        public double[] getHottestSpotTemp()
        {
            return this.hottestSpotTemp;
        }

        public double[] getTopOilTemp()
        {
            return this.topOilTemp;
        }

        public double[] getHotSpotTemp()
        {
            return this.hotSpotTemp;
        }

        public double[] getTau() 
        {
            return tauTO;
        }

        public double[] getFaa()
        {
            return Faa;
        }

        public double[] getAgingHour()
        {
            return Aging_hour;
        }

        public double[] getCumAging()
        {
            return Cum_Aging_hour;
        }

        public double getLossOfLife()
        {
            return LossOfLifeTotal;
        }

        public double[] getAmbientTempArr() 
        {
            return this.xfrmr.getAmbientTempArr();
        }

        public double getMaxPerUnit()
        {
            return this.maxPerUnit;
        }
    }

}
