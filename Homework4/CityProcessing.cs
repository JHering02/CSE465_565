using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Locations;

// Contains all the methods for creating LatLon.txt, CommonCityNames.txt, and CityStates.txt
namespace CityProcessing
{
    abstract class OutputOperation
    {
        private string zipcodes;
        private string outputFilePath;

        private Dictionary<string, Zipcode> states = new Dictionary<string, Zipcode>();

        public abstract void WriteToFile();
        public OutputOperation(string opName) {
            zipcodes = "./zipcodes.txt";
            outputFilePath = $"./{opName}.txt";
        }

        public void ProcessZipcodes()
        {
            using (StreamReader sr = new StreamReader(zipcodes))
            {
                // using Nullable<string>
                string? line;
                sr.ReadLine(); // we know the 1st line is the header for the data
                while ((line = sr.ReadLine()) != null)
                {
                    string[] lineDat = line.Split(' ');
                    long zip = long.Parse(lineDat[0]);
                    string cityName = lineDat[2];
                    string stateabbr = lineDat[3];
                    double lat = double.Parse(lineDat[5]);
                    double lon = double.Parse(lineDat[6]);
                    string cityText = lineDat[13];
                    states.Add(stateabbr, new Zipcode(zip));
                }

            }
        }
    }

    class LatLon : OutputOperation
    {
        public LatLon() : base("LatLon") { }
        // Overriding the ToString method to display the LatLon data.
        public override string ToString()
        {

            return "";
        }
        // Main method for preparing the Lat/Lon string data for the output file.
        private string[] prepLatLon() 
        {

            return new string[]{""};
        }
        // Writing the LatLon data to the output file.
        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

    class CommonCityNames : OutputOperation
    {
        public CommonCityNames() : base("CommonCityNames") {}
        
        public override string ToString()
        {
            return base.ToString();
        }

        private string[] prepCommonCities()
        {
            return new string[]{""};
        }

        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

    class CityStates : OutputOperation
    {
        public CityStates() : base("CityStates") { }

        public override string ToString()
        {
            return base.ToString();
        }

        private string[] prepCityStates()
        {
            return new string[]{""};
        }
        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

}

namespace Locations 
{
    // The State class will contain a data structure of Zipcodes with their respective lat/long coordinates. 
    //  TODO : override the multiplication operator to generate a new state only containing unique city names contained in both states.
    // public class State(string stateName)
    // {
    //     public string stateName { get; private set; } = stateName;
    //     private List<Zipcode> zipcodes = new List<Zipcode>();

    //     public void getZipcode(long zipcode)
    //     {
    //         zipcodes.Find(z => z.Equals(zipcode));
    //     }

    //     public static State operator == (State state1, State state2)
    //     {
    //         // return a new state with unique city names
    //     }

    //     public static State operator != (State state1, State state2)
    //     {
    //         // return a new state with unique city names
    //     }
    //     // public static State operator *(State state1, State state2)
    //     // {
    //     //     // return a new state with unique city names
    //     // }
    // }

    // Each Zipcode Belongs to a State, has a list of cities, which have a matrix array of lat/long coordinates.
    //  TODO : override the ToString method to display the city name and its coordinates.
    public class Zipcode(long zipcode)
    {
        public long Zip = zipcode;
        public List<City> cities = new List<City>();

        public void AddCity(string cityName, string cityText, double lat, double lon) {
            cities.Add(new City(cityName, lat, lon, cityText));
        }

        public void getCity(string cityName)
        {
            cities.Find(c => c.cityName.CompareTo(cityName) == 0);
        }

        public bool Equals(long other)
        {
            return this.Zip == other;
        }
        
    }

    public class City 
    {
        public string cityName { get; private set; }
        public string cityText { get; private set; }
        public double[,] latLon { get; private set; }
        public City(string cityName, double lat, double lon, string cityText)
        {
            this.cityName = cityName;
            this.latLon = new double[,]{{lat}, {lon}};
            this.cityText = cityText;
        }
    }
}