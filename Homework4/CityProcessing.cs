using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Locations;

/*
   CityProcessing.cs

   James Hering

   Note: The complexity of the solution is not due to me writing the code alone in base c# or using AI. I researched multiple data structures and how they operate in C#,
  setup .net core 8.0 with intellisense in vscode, and collaberated with my older brother (a c# developer) to review how I implemented all the requirements.

   14 April 2024

    This file contains the classes for processing the city data and writing the output files.
    There are three main operations being done here:

    1. LatLon - Writes the latitude and longitude of each given zipcode in zips.txt to a file by using the overidden == operator of the Zipcode class,
                and finding values in a list of zipcodes from 'zips.txt'.

    2. CommonCityNames - Writes only common city names between all states in a 'states.txt' file alphabetically using sorted HashSet and writes to a file.

    3. CityStates - Writes all the states that contain a cityname given by 'cities.txt' using the Any() part of a list, and sorts them to display
                    each state abbrebiation alphabetically separated by spaces to a file.

    List of features implemented throughout the code:

    Alias - Line 32 for list of Zipcodes
    Nullable - Used throughout various parts of the code to handle null cases
    Class/Struct Heirarchy - Used to create both CityProcessing and Locations namespaces (Subclasses in CityProcessing, Struct in Locations) (Lines 47, 207)
    Setter/Getter - Used in most of the classes to set and get the states/zipcodes/cities
    Overriding Operators - Used in the Zipcode class to set the comparison of zipcodes to only other long zipcode values (Lines 240, 255)
    Overriding ToString - Used in the Zipcode class to display the city name and its coordinates (Line 222)
    Overriding abstract class methods - Used in the OutputOperation class to write to the various output files (Lines 120, 141, 177)
    Matrix Array - Used in the City struct to store the latitude and longitude of a city (Line 264, mainly for proof of concept)
    Data Structures - Used SortedSet, HashSet, List, Dictionary, and Arrays to store and manipulate data (throughout the code)
    Delegates - Used in the Main method of Hw4 to write to the output files
    parameter variable types - Used throughout the code for different variables

    Pointers (NOT YET IMPLEMENTED) - Used in the SetStates method to set the states using C# pointers


*/

// Contains all the methods for creating LatLon.txt, CommonCityNames.txt, and CityStates.txt

// Alias used for a list of Zipcodes
using zipList = System.Collections.Generic.List<Locations.Zipcode>;

namespace CityProcessing
{
    public abstract class OutputOperation(string opName)
    {
        private readonly string zipcodes = "./zipcodes.txt";
        protected string outputFilePath = $"./{opName}.txt";
        protected static Dictionary<string, zipList> states = [];
        public abstract void WriteToFile();
        public void ProcessZipcodes()
        {
            using StreamReader sr = new(zipcodes);
            string? line;
            sr.ReadLine(); // we know the 1st line is the header for the data
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (line == null) continue;
                string[] lineDat = line.Split('\t');
                long zip = 0;
                double lat = 0;
                double lon = 0;
                long.TryParse(lineDat[1], out zip);
                double.TryParse(lineDat[6], out lat);
                double.TryParse(lineDat[7], out lon);
                string stateabbr = lineDat[4];
                string cityText = lineDat[13];
                string cityName = lineDat[3];

                if (states.TryGetValue(stateabbr, out zipList? zipList) && zipList.Exists(s => s == zip))
                {
                    zipList.Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
                else if (states.TryGetValue(stateabbr, out zipList))
                {
                    zipList.Add(new(zip));
                    zipList.Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
                else if (stateabbr != "")
                {
                    states[stateabbr] = [new(zip)];
                    states[stateabbr].Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
            }
        }


        public Dictionary<string, zipList> GetStates()
        {
            return states;
        }

        public void SetStates(Dictionary<string, zipList> newStates)
        {
            states = newStates;
        }

        // // Use C# Pointers to set the states
        // unsafe static void SetStates(Dictionary<string, zipList>* newStates)
        // {
        //     fixed (Dictionary<string, zipList>* pStates = &states)
        //     {
        //         *pStates = newStates;
        //     }
        // }
        
    }

    public class LatLon : OutputOperation
    {
        public LatLon() : base("LatLon") { }

        // Writing the LatLon data to the output file.
        public override void WriteToFile()
        {
            using StreamReader sr = new("./zips.txt");
            using StreamWriter sw = new(outputFilePath);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                foreach (var state in states)
                {
                    Zipcode? cityEntry = state.Value.Find(s => s == long.Parse(line));
                    if (cityEntry != null)
                        sw.WriteLine($"{cityEntry.GetCities()[0].LatLon[0, 0]} {cityEntry.GetCities()[0].LatLon[1, 0]}");
                }
            }
        }
    }

    public class CommonCityNames : OutputOperation
    {
        public CommonCityNames() : base("CommonCityNames") { }

        public override void WriteToFile()
        {
            using StreamReader sr = new("./states.txt");
            using StreamWriter sw = new(outputFilePath);
            string? line;
            SortedSet<string> commonCities = [];
            while ((line = sr.ReadLine()) != null)
            {
                if (commonCities.Count == 0 && states.TryGetValue(line, out zipList? value))
                {
                    foreach(var zip in value) 
                    {
                        commonCities.UnionWith(zip.GetCities().Select(c => c.CityName).ToHashSet());
                    }
                    continue;
                }
                else if (states.TryGetValue(line, out zipList? popValue))
                {
                    HashSet<string> cities = [];
                    foreach(var zip in popValue) 
                    {
                        cities.UnionWith(zip.GetCities().Select(c => c.CityName).ToHashSet());
                    }
                    commonCities.IntersectWith(cities);
                }
            }
            foreach(var city in commonCities)
            {
                sw.WriteLine(city.ToString());
            }
        }
    }

    public class CityStates : OutputOperation
    {
        public CityStates() : base("CityStates") { }
        public override void WriteToFile()
        {
            using StreamReader sr = new("./cities.txt");
            using StreamWriter sw = new(outputFilePath);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                List<string> commonStates = [];
                foreach (var state in states)
                {
                    if(state.Value.Any(s => s.GetCities().Any(cn => cn.CityName.Contains(line.ToUpper()))))
                        commonStates.Add(state.Key);
                }
                commonStates.Sort();
                int i = 0;
                do
                {
                    if (i != commonStates.Count - 1)
                        sw.Write($"{commonStates[i]} ");
                    else
                    sw.Write($"{commonStates[i]}");
                    i++;
                } while (i < commonStates.Count);
                sw.WriteLine();
            }
        }
    }

}

namespace Locations
{

    // Each Zipcode Belongs to a State, has a list of cities, which have a matrix array of lat/long coordinates.
    //  TODO : override the ToString method to display the city name and its coordinates.
    public class Zipcode(long zipcode)
    {
        private readonly long Zip = zipcode;
        private List<City> cities = [];

        public void AddCity(string cityName, string cityText, double lat, double lon)
        {
            cities.Add(new City(cityName, lat, lon, cityText));
        }

        public override string ToString()
        {
            string cityString = "";
            foreach (var city in cities)
            {
                cityString += $"{city.CityName} {city.LatLon[0, 0]} {city.LatLon[1, 0]}\n";
            }
            return cityString;
        }
        public List<City> GetCities()
        {
            return cities;
        }
        public long GetZipcode()
        {
            return Zip;
        }
        // OVERRIDING LOGICAL OPERATORS
        public static bool operator ==(Zipcode zipcode, long? other)
        {
            if ((object)zipcode == null)
                return other == null;
            return zipcode.Equals(other);
        }

        public static bool operator !=(Zipcode zipcode, long? other) => !(zipcode == other);

        public bool Equals(long? other)
        {
            
            return Zip == other;
        }
        public override bool Equals(object? obj) => Equals(obj as long?);
        public override int GetHashCode() => Zip.GetHashCode();
    }

    public struct City(string cityName, double lat, double lon, string cityText)
    {
        public string CityName { get; private set; } = cityName;
        public string CityText { get; private set; } = cityText;

        // USING MATRIX
        public double[,] LatLon { get; private set; } = new double[,] { { lat }, { lon } };
    }
}