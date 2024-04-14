/* 
  Homework#4

  James Hering
  
  Note: The complexity of the solution is not due to me writing the code alone in base c#. I researched multiple data structures and how they operate in C#,
  setup .net core 8.0 for intellisense, and collaberated with my older brother (a c# developer) to understand more ways to implement all the requirements.

  You are free to create as many classes within the Hw4.cs file or across 
  multiple files as you need. However, ensure that the Hw4.cs file is the 
  only one that contains a Main method. This method should be within a 
  class named hw4. This specific setup is crucial because your instructor 
  will use the hw4 class to execute and evaluate your work.
  */
  // BONUS POINT:
  // => Used Pointers in "CityProcessing.cs" from lines 10 to 15 <=
  // => Used Pointers in "CityProcessing.cs" from lines 40 to 63 <=

using System;
using System.IO;
using CityProcessing;
using Locations;

public class Hw4
{
    // Delegate Usage for writing to files
    public delegate void locationProcessor(OutputOperation op);
    public static void OpWrite(OutputOperation c)
    {
        c.WriteToFile();
    }

    public static void ProcessLocations(locationProcessor lp)
    {
        LatLon baseOp = new LatLon();
        baseOp.ProcessZipcodes();
        CommonCityNames commonOp = new CommonCityNames();
        commonOp.SetStates(baseOp.GetStates());
        CityStates stateOp = new CityStates();
        stateOp.SetStates(baseOp.GetStates());
        lp(baseOp);
        lp(commonOp);
        lp(stateOp);
    }
    public static void Main(string[] args)
    {
        // Capture the start time
        // Must be the first line of this method
        DateTime startTime = DateTime.Now; // Do not change
        // ============================
        // Do not add or change anything above, inside the 
        // Main method
        // ============================
        try {
            // Call Constructors for each class and process those zipcodes
            ProcessLocations(OpWrite);
        } catch (FileNotFoundException e) {
            Console.WriteLine(e.Message);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
        // ============================
        // Do not add or change anything below, inside the 
        // Main method
        // ============================

        // Capture the end time
        DateTime endTime = DateTime.Now;  // Do not change
        
        // Calculate the elapsed time
        TimeSpan elapsedTime = endTime - startTime; // Do not change
        
        // Display the elapsed time in milliseconds
        Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms");
    }
}