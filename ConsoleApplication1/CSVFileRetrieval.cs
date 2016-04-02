using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/*
 * *************************************
 * FileRetrieval.cs
 * Created by Abner Joel Hernandez
 * Date: 2/13/2016
 * Version 0.1
 * 
 * ************************************
 */

// ../../plltest.csv


namespace HeatRunAnalysis
{

    class CSVFileRetrieval
    {

//*******************************************************MEMBER VARIABLES*************************************************************
       
        private string filePath; // The file path given to the user
        List<double> loadProfile = new List<double>(); // This will store the loading profile






//*********************************************************CONSTRUCTOR****************************************************************
//************************************Constructor for FileRetrival. Asks for the path and retrieval mode******************************
        public CSVFileRetrieval(string filePath)
        {
            this.filePath = filePath;
            //this.retrievalMode = retrievalMode;
           // setIntervalLength(retrievalMode);
            retrieveFileInfo();
        }



//**********************************************************METHODS*******************************************************************
//***************************************This function stores the information from the file*******************************************
        private void retrieveFileInfo()
        {

            // Program will attempt to retrieve the file
            try
            {
                // Create a stream reader class to read the file path
                StreamReader sr = new StreamReader(@filePath);
                // Since it will store the values into a string, make a list of strings
                List<string> data = new List<string>();

                // Loops until there is no more elements in the file
                while (!sr.EndOfStream)
                {
                    data.Add(sr.ReadLine());
                }

                // Assign a temporary variable to compare to later
                double temp;
                // Convert Data to double
                for (int i = 0; i < data.Count; i++)
                {
                    /* If there is a string, it will not store it
                    It will only store things that can convert into a double*/
                    if (!double.TryParse(data[i], out temp))
                        continue;

                    loadProfile.Add(Convert.ToDouble(data[i]));
                }

                sr.Close();

            }
            // Catch the FileNotFoundException if thrown
            catch (FileNotFoundException e)
            {
                // FileNotFoundExceptions are handled here.
                //Displays that it could not find the file
                Console.WriteLine(e.Message);
                Console.ReadKey();

            }
            // Catch the IOException if thrown
            catch (IOException e)
            {
                /* Extract some information from this exception, and then 
                   throw it to the parent method. */
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                Console.ReadKey();
                throw;
            }
            catch  
            {
                //Console.WriteLine("Invalid path. Try again");
                
            }

            
        }




//**********************************************************GETTERS******************************************************************
        public double[] getArray()
        {
            return loadProfile.ToArray(); 
        }
      

    }
}
