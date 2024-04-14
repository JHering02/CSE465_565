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

        private Dictionary<string, List<Zipcode>> states = new Dictionary<string, List<Zipcode>>();

        public abstract void WriteToFile();
        public OutputOperation(string opName) {
            zipcodes = "./zipcodes.txt";
            outputFilePath = $"./{opName}.txt";
        }

        public void ProcessZipcodes()
        {
            using (StreamReader sr = new StreamReader(zipcodes))
            {
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
                    if (states.TryGetValue(stateabbr, out List<Zipcode>? value) && value.Exists(s => s.Equals(zip)))
                    {
                        value.Find(s => s.Equals(zip)).AddCity(cityName, cityText, lat, lon);
                    }
                    else if (states.TryGetValue(stateabbr, out List<Zipcode>? value2) && !value2.Exists(s => s.Equals(zip)))
                    {
                        value2.Add(new Zipcode(zip));
                        value2.Find(s => s.Equals(zip)).AddCity(cityName, cityText, lat, lon);
                    }
                    else
                    {
                        states.Add(stateabbr, new List<Zipcode>(){new Zipcode(zip)});
                        states[stateabbr].Find(s => s.Equals(zip)).AddCity(cityName, cityText, lat, lon);
                    }
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

    // Each Zipcode Belongs to a State, has a list of cities, which have a matrix array of lat/long coordinates.
    //  TODO : override the ToString method to display the city name and its coordinates.
    public class Zipcode(long zipcode)
    {
        private long Zip = zipcode;
        public List<City> cities = new List<City>();

        public void AddCity(string cityName, string cityText, double lat, double lon) {
            cities.Add(new City(cityName, lat, lon, cityText));
        }

        public void getCity(string cityName)
        {
            cities.Find(c => c.cityName.CompareTo(cityName) == 0);
        }

        public long getZipcode() {
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
        
        public bool Equals(long other)
        {
            if (this == null)
            {
                return false;
            }
            return Zip == other;
        }
        public override bool Equals(object obj) => Equals(obj as long?);
        public override int GetHashCode() => Zip.GetHashCode();
    }

    public class City 
    {
        public string cityName { get; private set; }
        public string cityText { get; private set; }

        // USING MATRIX
        public double[,] latLon { get; private set; }
        public City(string cityName, double lat, double lon, string cityText)
        {
            this.cityName = cityName;
            this.latLon = new double[,]{{lat}, {lon}};
            this.cityText = cityText;
        }
    }
}