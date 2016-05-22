﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatRunAnalysisTool
{

    /*
    * *************************************
    * NormalLoadingLimit.cs
    * Created by  Aaron Bautista
    * Modified by Abner Joel Hernandez
    * Date: 2/13/2016
    * Version 0.2
    * 
    * ************************************
    */

    class NormalLoadingLimit
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
        private double[] topOilTemp2;
        private double[] hottestSpotTemp;

        private SubstationTransformer xfrmr; // Store transformer characteristics

        private double[] tauTO; // TauO

        private int t; // Time interval. 1 is hour. 0.5 is hald an hour


        public NormalLoadingLimit() { }

        public NormalLoadingLimit(double[] perUnitValues, SubstationTransformer xfrmr)
        {

            this.perUnitValues = perUnitValues;
            this.xfrmr = xfrmr;

            calculateKRMS();

            ultimateTopOil = new double[perUnitValues.Length];
            hotSpotTemp = new double[perUnitValues.Length];
            topOilTemp = new double[perUnitValues.Length];
            topOilTemp2 = new double[perUnitValues.Length];
            hottestSpotTemp = new double[perUnitValues.Length];


            calculateUltimateTopOil();

            // Check to see if we need to calculate tauTO or if n = 1 the tauTO = tauTR
            calculateTauTO();



            calculateTopOilTemp1();
            calculateTopOilTemp2();
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

                    //Console.WriteLine("This should be 18.14 : " +topOilUltimate);
                    ultimateTopOil[i] = topOilUltimate;
                    continue;
                }
                // Calculate top Oil Ultimate first then calculate TOP oil
                topOilUltimate = (xfrmr.getdeltaThetaTO_R()) * Math.Pow(((perUnitValues[i - 1] * perUnitValues[i - 1]
                                  * xfrmr.getR()) + 1) / (xfrmr.getR() + 1), xfrmr.getN());

                ultimateTopOil[i] = topOilUltimate;
            }

            for (int i = 0; i < topOilTemp.Length; i++)
            {
               // Console.WriteLine(ultimateTopOil[i]);

            }

        }

        private void calculateTopOilTemp1()
        {

            for (int i = 0; i < topOilTemp.Length; i++)
            {
                if (i == 0)
                {
                    topOilTemp[0] = ultimateTopOil[0];
                    continue;
                }

                topOilTemp[i] = Math.Round((ultimateTopOil[i] * (1 - Math.Exp(-1 / tauTO[i])))
                                               + (topOilTemp[i] * Math.Exp(-1 / tauTO[i])), 2);

            }

        }

        private void calculateTopOilTemp2()
        {
            for (int i = 0; i < topOilTemp.Length; i++)
            {
                if (i == 0)
                {
                    topOilTemp2[i] = (ultimateTopOil[i] * (1 - Math.Exp(-1 / tauTO[i])))
                                               + (topOilTemp[topOilTemp.Length - 1] * Math.Exp(-1 / tauTO[i]));
                    //topOilTemp2[i] = topOilTemp[topOilTemp2.Length - 1];
                    continue;
                }

                topOilTemp2[i] = (ultimateTopOil[i] * (1 - Math.Exp(-1 / tauTO[i])))
                                               + (topOilTemp2[i - 1] * Math.Exp(-1 / tauTO[i]));

            }



        }

        private void calculateHotSpotTemp()
        {
            // Calculate Hot Spot Temperature for each load
            // Equation: DetaThetaHS = deltaThetaH,R * (K ^ 2m)

            for (int i = 0; i < perUnitValues.Length; i++)
            {
                hotSpotTemp[i] = xfrmr.getdeltaThetaHS_R() * Math.Pow(perUnitValues[i], 2 * xfrmr.getM());

            }

        }

        private void calculateHottestSpotTemp()
        {
            for (int i = 0; i < hottestSpotTemp.Length; i++)
            {
                hottestSpotTemp[i] = Math.Round(topOilTemp2[i] + hotSpotTemp[i] + xfrmr.getAmbientTemp(), 2);
            }

        }

        private void calculateKRMS()
        {
            double krmsstore = Math.Sqrt((Math.Pow(perUnitValues[perUnitValues.Length - 1], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 2], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 3], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 4], 2) +
                                    Math.Pow(perUnitValues[perUnitValues.Length - 5], 2) + Math.Pow(perUnitValues[perUnitValues.Length - 6], 2)) / 6);
            // To change later
            this.kRMS = krmsstore;
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

                    if (i != 0)
                    {
                        // Not initial
                        if (ultimateTopOil[i] == ultimateTopOil[i - 1])
                        {
                            // Prev is same 
                            tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[i + 1] / xfrmr.getTauTO_R()) - (ultimateTopOil[i - 2] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[i + 1] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i - 2] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));
                            Console.WriteLine("Initial reached " + i);
                            Console.WriteLine("Ultimates: " + ultimateTopOil[i + 1] + " , " + ultimateTopOil[i - 2]);
                            continue;
                        }

                        else if (i == tauTO.Length - 1)
                        {
                            tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[0] / xfrmr.getTauTO_R()) - (ultimateTopOil[i] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[0] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));

                            continue;
                        }



                        tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[i + 1] / xfrmr.getTauTO_R()) - (ultimateTopOil[i] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[i + 1] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));


                    }
                    else
                    {
                        // Initial
                        tauTO[i] = xfrmr.getTauTO_R() * ((ultimateTopOil[i + 1] / xfrmr.getTauTO_R()) - (ultimateTopOil[i] / xfrmr.getTauTO_R())) / (Math.Pow(ultimateTopOil[i + 1] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()) - Math.Pow(ultimateTopOil[i] / xfrmr.getTauTO_R(), 1 / xfrmr.getN()));
                    }

                }


            }
        }


        public void printInfo()
        {
            Console.WriteLine();
            Console.WriteLine("NORMAL (Load Cycles and Temperature Rises for " + xfrmr.getSubstationName());
            Console.WriteLine();

            for (int i = 0; i < perUnitValues.Length; i++)
            {
                Console.WriteLine("LOAD HOUR: " + (i + 1) + "\tLOAD PU: "
                    + perUnitValues[i] + "\tTOP OIL TEMP: " + topOilTemp[i] + "\tTOP OIL TEMP2: " +
                        topOilTemp2[i] + "\tHOT SPOT TEMP: " + hotSpotTemp[i] + "\t  HOTTEST SPOT TEMP: " + hottestSpotTemp[i] + "\t Tau " + tauTO[i]);
            }

        }


        //**********************************************************GETTERS******************************************************************

        public double[] getHottestSpotTemp()
        {
            return this.hottestSpotTemp;
        }

        public double[] getTopOilTemp() 
        {
            return topOilTemp2;
        }

        public double[] getHotSpotTemp() 
        {
            return hotSpotTemp;
        }


    }
}