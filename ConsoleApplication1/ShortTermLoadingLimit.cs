﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatRunAnalysis
{
    class ShortTermLoadingLimit
    {

        //This will store our perunit values
        double[] perUnitValues;

        // This will store our load hours
        // Since we know the hours and it is going
        //to be hours 0-3, we can define it

        // This will store our hotspot temp
        private double[] ultimateTopOil; // Ultimate top oil rise. Used for top oil and tauO calculations
        double[] hotSpotTemp;
        double[] topOilTemp;
        double[] hottestSpotTemp;

        private double kRMS;

        SubstationTransformer xfrmr;


        public ShortTermLoadingLimit() { }

        public ShortTermLoadingLimit(double[] perUnitValues, SubstationTransformer xfrmr)
        {
            this.perUnitValues = perUnitValues;
            this.xfrmr = xfrmr;

            calculateKRMS();

            ultimateTopOil = new double[perUnitValues.Length];
            hotSpotTemp = new double[perUnitValues.Length];
            topOilTemp = new double[perUnitValues.Length];
            hottestSpotTemp = new double[perUnitValues.Length];

            // Check to see if we need to calculate tauTO or if n = 1 the tauTO = tauTR
            //calculateTauTO();

            calculateUltimateTopOil();

            calculateTopOilTemp();
            calculateHotSpotTemp();
            calculateHottestSpotTemp();
        }


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
                if (i == 0)
                {
                    topOilTemp[i] = 26.86;
                }
                    // if per unit load is at max then must find formula for top oil
                else if (i == 8)
                {
                    topOilTemp[i] = Math.Round((7.42 * 1.29 * 1.29) + 1.53 + (0.75 * 45.19), 2);
                }
                else if (i != 8 || i != 0)
                {
                    // top-oil temperature
                    topOilTemp[i] = Math.Round((7.42 * perUnitValues[i - 1] * perUnitValues[i - 1]) + 1.53 + (0.75 * topOilTemp[i - 1]), 2);
                }
            }

        }

        private void calculateHotSpotTemp()
        {
            for (int i = 0; i < perUnitValues.Length; i++)
            {
                // hot spot temperature
                if (i == 7)
                {
                    hotSpotTemp[i] = Math.Round(28.6 * 1.17 * 1.17, 2);
                }
                else if (i != 7)
                {
                    if (hottestSpotTemp[i] > 140)
                    {
                        hotSpotTemp[i] = Math.Round(28.6 * perUnitValues[i - 1] * perUnitValues[i - 1], 2);
                    }

                    else if (hottestSpotTemp[i] < 140)
                    {
                        hotSpotTemp[i] = Math.Round(28.6 * perUnitValues[i] * perUnitValues[i], 2);
                    }
                }

            }
        }

        private void calculateHottestSpotTemp()
        {
            for (int i = 0; i < hottestSpotTemp.Length; i++)
            {
                hottestSpotTemp[i] = Math.Round(topOilTemp[i] + hotSpotTemp[i] + 30, 2);
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


        // Prints the info
        public void printInfo()
        {
            Console.WriteLine();
            Console.WriteLine("STELL (Load Cycles and Temperature Rises for KRAMER");
            Console.WriteLine();

            for (int i = 0; i < hottestSpotTemp.Length; i++)
            {
                Console.WriteLine("LOAD HOUR: " + (i + 1)+ "\tLOAD PU: "
                    + perUnitValues[i] + "\tTOP OIL TEMP: " + topOilTemp[i] + "\tHOT SPOT TEMP: " + hotSpotTemp[i] + "\tHOTTEST SPOT TEMP: " + hottestSpotTemp[i]);
            }
        }

        //**********************************************************GETTERS******************************************************************

        public double[] getHottestSpotTemp()
        {
            return this.hottestSpotTemp;
        }

    }
}