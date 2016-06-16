using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatRunAnalysisTool
{
    class LossOfLife
    {
        // Loss of life important arrays
        double[] Load_PU;
        double[] Hot_spot_temp;

        // Loss of life
        double[] Faa;
        double[] Aging_hour;
        double[] Cum_Aging_hour;

        double Loss_Of_Life;

        // Constructors
        public LossOfLife(){}

        // Contructor
        public LossOfLife(double[] Load_PU, double[] Hot_spot_temp)
        {
            // Load Per Unit
            this.Load_PU = Load_PU;
            this.Hot_spot_temp = Hot_spot_temp;

            // Instantiate Arrays
            this.Faa = new double[Load_PU.Length];
            this.Aging_hour = new double[Load_PU.Length];
            this.Cum_Aging_hour = new double[Load_PU.Length];

            calculateFaa();
            calculateCumAgingHour();
            calculateLossOfLife();
        }


        private void calculateFaa()
        {

            for (int i = 0; i < Faa.Length; i++)
            {
                Faa[i] = Math.Exp((39.16449 - (15000 / (273 + Hot_spot_temp[i])) ));
                Aging_hour[i] = Faa[i];
            }
        
        }

        private void calculateCumAgingHour()
        {

            for (int i = 0; i < Cum_Aging_hour.Length; i++)
            {
                if(i == 0)
                {
                    Cum_Aging_hour[i] = Aging_hour[i]; 
                    continue;
                }
                Cum_Aging_hour[i] = Cum_Aging_hour[i - 1] + Aging_hour[i];
            }
        
        }


        private void calculateLossOfLife()
        {
            this.Loss_Of_Life =  Math.Round(100 * Cum_Aging_hour[Cum_Aging_hour.Length - 1] / 180000 , 3 );
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
           return Loss_Of_Life;
       }


    }
}
