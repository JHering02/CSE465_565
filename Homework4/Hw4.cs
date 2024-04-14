/* 
  Homework#4

  Add your name here: James Hering

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

public class Hw4
{
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
            CommonCityNames commonCityNames = new CommonCityNames();
            commonCityNames.ProcessZipcodes();
            commonCityNames.WriteToFile();

            CityStates cityStates = new CityStates();
            cityStates.ProcessZipcodes();
            cityStates.WriteToFile();

            // LatLon latLon = new LatLon();
            // latLon.ProcessZipcodes();
            // latLon.WriteToFile();
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