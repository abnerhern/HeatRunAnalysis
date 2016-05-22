using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatRunAnalysisTool
{
    class LossOfLife
    {
        double[] Load_PU = new double[24];
        double[] Hot_spot_temp = new double[24];
        double[] Faa = new double[24];
        double[] Aging_hour = new double[24];
        double[] Cum_Aging_hour = new double[24];
        double Loss_Of_Life;

        public LossOfLife(){}


        public LossOfLife(double[] Load_PU, double[] Hot_spot_temp)
        {
            this.Load_PU = Load_PU;
            this.Hot_spot_temp = Hot_spot_temp;
            calculateFaa();
            calculateCumAgingHour();
            calculateLossOfLife();
        }


        private void calculateFaa()
        {

            for (int i = 0; i < 23; i++)
            {
                Faa[i] = Math.Exp((39.16449 - 15000 / (273 + Hot_spot_temp[i])));
                Faa[i] = Math.Round(Faa[i], 3);
                Aging_hour[i] = Faa[i];
            }
        
        }

        private void calculateCumAgingHour()
        {
            for (int i = 0; i < 23; i++)
            {
                Cum_Aging_hour[i + 1] = Cum_Aging_hour[i] + Aging_hour[i + 1];
                Cum_Aging_hour[i + 1] = Math.Round(Cum_Aging_hour[i + 1], 3);
            }
        
        }


        private void calculateLossOfLife()
        {
            this.Loss_Of_Life =  Math.Round(24 * 100 * Cum_Aging_hour[23] / 180000 ,3 );
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


        public void printInfo() 
        {
            Console.WriteLine("Hour\tLoad\tHS Temp\t  Aging Accl. Factor\tAging Hour\tCummulative Aging Hour");
            for (int i = 0; i < 24; i++)
            {
                Console.WriteLine((i + 1) + "\t" + Load_PU[i] + "\t" + Hot_spot_temp[i] + "\t\t" + Faa[i] + "\t\t" + Aging_hour[i] + 
                    "\t\t" + Cum_Aging_hour[i]);
            }
            Console.WriteLine("Loss of life: " + Loss_Of_Life + "%");   
        }


    }
}
