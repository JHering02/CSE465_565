using System.IO;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Locations;

// Contains all the methods for creating LatLon.txt, CommonCityNames.txt, and CityStates.txt
namespace CityProcessing
{
    abstract class OutputOperation(string opName)
    {
        private readonly string zipcodes = "./zipcodes.txt";
        protected string outputFilePath = $"./{opName}.txt";

        protected Dictionary<string, List<Zipcode>> states = [];

        public abstract void WriteToFile();

        public void ProcessZipcodes()
        {
            using StreamReader sr = new(zipcodes);
            // USING NULLABLE
            string? line;
            sr.ReadLine(); // we know the 1st line is the header for the data
            while ((line = sr.ReadLine()) != null)
            {
                string[] lineDat = line.Split(' ');
                long zip = long.Parse(lineDat[0]);
                string stateabbr = lineDat[3];
                double lat = double.Parse(lineDat[5]), lon = double.Parse(lineDat[6]);
                string cityText = lineDat[13], cityName = lineDat[2];
                if (states.TryGetValue(stateabbr, out List<Zipcode>? value) && value.Exists(s => s == zip))
                {
                    value.Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
                else if (states.TryGetValue(stateabbr, out List<Zipcode>? value2) && !value2.Exists(s => s == zip))
                {
                    value2.Add(new Zipcode(zip));
                    value2.Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
                else if (!states.ContainsKey(stateabbr))
                {
                    states.Add(stateabbr, [new(zip)]);
                    states[stateabbr].Find(s => s == zip)?.AddCity(cityName, cityText, lat, lon);
                }
            }
        }
    }

    class LatLon : OutputOperation
    {
        public LatLon() : base("LatLon") { }

        // Writing the LatLon data to the output file.
        public override void WriteToFile()
        {
            StreamReader sr = new("./zips.txt");
            StreamWriter sw = new(outputFilePath);
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

    class CommonCityNames : OutputOperation
    {
        public CommonCityNames() : base("CommonCityNames") { }

        public override void WriteToFile()
        {
            StreamReader sr = new("./states.txt");
            StreamWriter sw = new(outputFilePath);
            string? line;
            List<string> commonCities = [];
            while ((line = sr.ReadLine()) != null)
            {
                if (commonCities.Count == 0 && states.TryGetValue(line, out List<Zipcode>? value))
                {
                    foreach(var zip in value) 
                    {
                        commonCities.AddRange(zip.GetCities().Select(c => c.CityName));
                    }
                    continue;
                }
                else if (states.TryGetValue(line, out List<Zipcode>? newValue))
                {
                    List<string> cities = [];
                    foreach(var zip in newValue) 
                    {
                        cities.AddRange(zip.GetCities().Select(c => c.CityName));
                    }
                    commonCities.Intersect(cities).ToList();
                }
            }
        }
    }

    class CityStates : OutputOperation
    {
        public CityStates() : base("CityStates") { }
        public override void WriteToFile()
        {
            StreamReader sr = new("./zips.txt");
            StreamWriter sw = new(outputFilePath);
            string? line;
            List<string> commonStates = [];
            while ((line = sr.ReadLine()) != null)
            {
                foreach (var state in states)
                {
                    if(state.Value.Any(s => s.GetCities().Any(cn => cn.CityName.CompareTo(line) == 0)))
                        commonStates.Add(state.Key);
                }
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
        public static bool operator ==(Zipcode zipcode1, long? other)
        {
            if (zipcode1 == null)
            {
                return false;
            }
            return zipcode1.Equals(other);
        }

        public static bool operator !=(Zipcode zipcode1, long? other) => !(zipcode1 == other);

        public bool Equals(long? other)
        {
            if (this == null || other == null)
            {
                return false;
            }
            return Zip == other;
        }
        public override bool Equals(object obj) => Equals(obj as long?);
        public override int GetHashCode() => Zip.GetHashCode();
    }

    public class City(string cityName, double lat, double lon, string cityText)
    {
        public string CityName { get; private set; } = cityName;
        public string CityText { get; private set; } = cityText;

        // USING MATRIX
        public double[,] LatLon { get; private set; } = new double[,] { { lat }, { lon } };
    }
}