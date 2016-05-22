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

        private double t; // Time interval. 1 is hour. 0.5 is hald an hour

        private SubstationTransformer xfrmr; // Store transformer characteristics

        private double threshold; //Stores the threshold value for the maximum

        private double maxPerUnit; // Stores the maximim perunit value possible for this loading limit


        public LoadingLimit() { }

        public LoadingLimit(double[] perUnitValues, SubstationTransformer xfrmr, double threshold, double t)
        {
            // Store the values of the array, transformer characerstics and the threshold values
            this.perUnitValues = perUnitValues;
            this.xfrmr = xfrmr;
            this.threshold = threshold;
            this.t = t;


            calculateKRMS();

            // Sets the array lengths for each array
            ultimateTopOil = new double[perUnitValues.Length + 1];
            hotSpotTemp = new double[perUnitValues.Length];
            topOilTemp= new double[perUnitValues.Length];
            hottestSpotTemp = new double[perUnitValues.Length];
            tauTO = new double[perUnitValues.Length];
            

            // Check to see if we need to calculate tauTO or if n = 1 the tauTO = tauTR
            //calculateTauTO();

            calculateUltimateTopOil();

            calculateTopOilTemp();
            calculateHotSpotTemp();
            calculateHottestSpotTemp();
        
        }

        //**************************************************************METHODS***************************************************************

        private void calculateUltimateTopOil()
        {
            double topOilUltimate = 0;

            for (int i = 0; i <= topOilTemp.Length; i++)
            {
                if (i == 0)
                {
                    // For the first iteration, the RMS value is treated as the current value for initial value.
                    topOilUltimate = (xfrmr.getdeltaThetaTO_R()) *
                    Math.Pow(((kRMS * kRMS * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());


                    // Now that we have the initial values, we can get the top oil values the first iteration
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
                if (i == 0)
                {
                    topOilTemp[0] = ultimateTopOil[1];
                    continue;
                }

                topOilTemp[i] = Math.Round((ultimateTopOil[i] * (1 - Math.Exp(-t / tauTO[i])))
                                               + (topOilTemp[i - 1] * Math.Exp(-t / tauTO[i])), 2);

            }

        }

        private void calculateHotSpotTemp()
        {
            for (int i = 0; i < perUnitValues.Length; i++)
            {
                if (hottestSpotTemp[i] > threshold)
                {
                    hotSpotTemp[i] = Math.Round(xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i - 1], 2 * xfrmr.getM()), 2);
                }

                else if (hottestSpotTemp[i] < threshold)
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
                for (int i = 0; i < tauTO.Length; i++)
                {

                    tauTO[i] = xfrmr.getTauTO_R();
                }
            }
            else
            {
                // Todo

            }
        }


        //**********************************************************GETTERS******************************************************************

        public double[] getHottestSpotTemp()
        {
            return this.hottestSpotTemp;
        }

    }
}
