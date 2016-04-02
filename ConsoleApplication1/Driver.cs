using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* *************************************
* Driver.cs
* Created by Abner Joel Hernandez
* Date: 2/18/2016
* Version 0.2
* 
* ************************************
*/


namespace HeatRunAnalysis
{
    class Driver
    {

        static void Main(string[] args)
        {
            // Initialize Objects here
            LoadMultiplier loadProfiles = new LoadMultiplier();
            SubstationTransformer xfrmr = new SubstationTransformer("Kramer", 186.666, 30);
            xfrmr.fillMoreInfo(4, 28.6 , 36.0, 4.87);
            xfrmr.setTauTO_R(3.5);

            NormalLoadingLimit nll = new NormalLoadingLimit();
            PlannedLoadingLimit pll = new PlannedLoadingLimit();
            LongTermLoadingLimit ltell = new LongTermLoadingLimit();
            ShortTermLoadingLimit stell = new ShortTermLoadingLimit(); 
            LossOfLife nla = new LossOfLife();
            LossOfLife pla = new LossOfLife();
            LossOfLife lla = new LossOfLife();
            LossOfLife sll = new LossOfLife();

            // Initially start with a negative number
            int userIn = -1;


           
            // Loop main menu
            do
            {
                // Display Main Menu
                printMainMenu();
                // Set the user in to what the user sets it to
                userIn = getUserIn(userIn);
                
                // Switch statement to control what is happening.
                switch (userIn) 
                {
                    case 1:
                        loadProfiles = getFile();
                        break;
                    case 2:
                        xfrmr = getTransformerInfo();
                        break;
                    case 3:
                        nll = new NormalLoadingLimit(loadProfiles.getNormalLoadProfile(), xfrmr);
                        pll = new PlannedLoadingLimit(loadProfiles.getPLLLoadProfile(), xfrmr);
                        ltell = new LongTermLoadingLimit(loadProfiles.getLTELLLoadProfile(), xfrmr);
                        stell = new ShortTermLoadingLimit(loadProfiles.getSTELLoadProfile(), xfrmr);

                        nla = new LossOfLife(loadProfiles.getNormalLoadProfile(), nll.getHottestSpotTemp());
                        pla = new LossOfLife(loadProfiles.getPLLLoadProfile(), pll.getHottestSpotTemp());
                        lla = new LossOfLife(loadProfiles.getLTELLLoadProfile(), ltell.getHottestSpotTemp());
                        sll = new LossOfLife(loadProfiles.getSTELLoadProfile(), stell.getHottestSpotTemp());

                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case 4:
                        printLoadProfiles(loadProfiles);
                        break;
                    case 5:
                        Console.WriteLine("Normal Loading Analysis: ");
                        nll.printInfo();
                        Console.WriteLine("Planned Loading Analysis: ");
                        pll.printInfo();
                        Console.WriteLine("Long Term Loading Analysis: ");
                        ltell.printInfo();
                        Console.WriteLine("Short Loading Analysis: ");
                        stell.printInfo();

                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();

                        break;
                    case 6:
                        Console.WriteLine("Normal Loading Loss of Life: ");
                        printLossOfLife(nla);
                        Console.WriteLine("Planned Loading Loss of Life: ");
                        printLossOfLife(pla);
                        Console.WriteLine("Long Term Loading Loss of Life: ");
                        printLossOfLife(lla);
                        Console.WriteLine("Short Loading Loss of Life: ");
                        printLossOfLife(sll);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case 7:
                        break;
                    default:
                        Console.WriteLine("That is not a choice.");
                    break;

                    
                }     
            Console.Clear();
            }
            while(userIn != 7);
         
        }

        private static void printMainMenu()
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("\t1. Input file");
            Console.WriteLine("\t2. Input Substation Transformer Parameters");
            Console.WriteLine("\t3. Calculate Loading Limits and Loss of Life");
            Console.WriteLine("\t4. Print Loading Limits");
            Console.WriteLine("\t5. Print Hot Spot of Loading Limits");
            Console.WriteLine("\t6. Print Loss of Life");
            Console.WriteLine("\t7. Exit Program");
        
        }

        private static int getUserIn(int userIn) 
        {
            string userString;

            try
            {
                Console.Write("Enter a choice: ");
                userString = Console.ReadLine();
                return Convert.ToInt32(userString);

            }
            catch
            {
                Console.WriteLine("Invalid Entry. Try again.");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return -1;
            }
           
            

        }
       
        private static LoadMultiplier getFile()
        {
            string userIn = "";
            Console.WriteLine("Enter the path to the load profile");
            userIn = Console.ReadLine();

            CSVFileRetrieval f = new CSVFileRetrieval(userIn);
            double[] K;
            K = f.getArray();

            try
            {
                LoadMultiplier l = new LoadMultiplier(K, 2);

                Console.WriteLine("File Retrival Successful.");

                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return l;
            }
            catch 
            {
                Console.WriteLine("Could not retrieve load profiles for loading limits. Check path?");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                LoadMultiplier l = new LoadMultiplier();
                return l;
            }
            
           
        }

        private static SubstationTransformer getTransformerInfo() 
        {
            string[] userIn1 = new string[3];
            string[] userIn2 = new string[5];


            Console.Write("Enter the Substation Name: ");
            userIn1[0] = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Enter the MVA Rating: ");
            userIn1[1] = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Enter the Ambient Temperature: ");
            userIn1[2] = Console.ReadLine();
            Console.WriteLine();

            SubstationTransformer xfrmr = new SubstationTransformer(userIn1[0], Convert.ToDouble(userIn1[1]), Convert.ToDouble(userIn1[2]));

            Console.Write("Enter the Cooling Mode.\nONAN = 1, ONAF = 2, Non-Directed OFAF or OFWF = 3, Directed ODAF or ODWF = 4 ");
            userIn2[0] = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Enter the Hottest-spot conductor rise over top-oil temperature, at rated load (ΔΘHS,R): ");
            userIn2[1] = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Enter the Top Oil Rise over ambient at rated load (ΔΘTO,R): ");
            userIn2[2] = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Enter the ratio of load loss at rated load to no-load loss (R): ");
            userIn2[3] = Console.ReadLine();
            Console.WriteLine();


            xfrmr.fillMoreInfo(Convert.ToInt32(userIn2[0]), Convert.ToDouble(userIn2[1]),
                               Convert.ToDouble(userIn2[2]), Convert.ToDouble(userIn2[3]));

            string userIn3;
            Console.Write("Do you have Oil thermal time constant for rated load "
                + "(TauTO_R) or would you like to calculate it? Y to Calculate ,  N to input : ");

            userIn3 = Console.ReadLine();

            if (userIn3 == "Y" || userIn3 == "y")
            {
                string[] userIn4 = new string[4];
                
                Console.Write("Enter the Core Coil Weight: ");
                userIn4[0] = Console.ReadLine();
                Console.WriteLine();

                Console.Write("Enter the Tank weight: ");
                userIn4[1] = Console.ReadLine();
                Console.WriteLine();

                Console.Write("Enter the Oil Volume: ");
                userIn4[2] = Console.ReadLine();
                Console.WriteLine();
                
                Console.Write("Enter the loss in watts at rated load (PT,R): ");
                userIn4[3] = Console.ReadLine();
                Console.WriteLine();

                xfrmr.fillMoreInfoTwo(Convert.ToDouble(userIn4[0]), Convert.ToDouble(userIn4[1]), Convert.ToDouble(userIn4[2]), Convert.ToDouble(userIn4[3]));
            }
            else
            {  
                Console.Write("Enter the Oil thermal time constant for rated load (TauTO_R): ");
                userIn3 = Console.ReadLine();

                xfrmr.setTauTO_R(Convert.ToDouble(userIn3));
            }

            return xfrmr;
        }

        private static void printLoadProfiles(LoadMultiplier l)
        {
            Console.WriteLine("Hour\tNormal\tPLL\tLTELL\tSTELL");

            for (int i = 0; i < l.getPLLLoadProfile().Length; i++ )
            {
                Console.WriteLine( (i+1) + "\t" + l.getNormalLoadProfile()[i] + "\t" + 
                    l.getPLLLoadProfile()[i] + "\t" + l.getLTELLLoadProfile()[i] +
                    "\t" + l.getSTELLoadProfile()[i]);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private static void printLossOfLife(LossOfLife l)
        {
            l.printInfo();
        }

    }
}
