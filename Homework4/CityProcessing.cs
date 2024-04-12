
// Contains all the methods for creating LatLon.txt, CommonCityNames.txt, and CityStates.txt
 namespace CityProcessing
{
    abstract class OutputOperation
    {
        public abstract void ReadFromFile();
        public abstract void WriteToFile();      
    }

    class LatLon : OutputOperation
    {
        public override void ReadFromFile()
        {
            throw new NotImplementedException();
        }

        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

    class CommonCityNames : OutputOperation
    {
        public override void ReadFromFile()
        {
            throw new NotImplementedException();
        }

        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

    class CityStates : OutputOperation
    {
        public override void ReadFromFile()
        {
            throw new NotImplementedException();
        }

        public override void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }

}

namespace Locations {
    // The State class will contain a data structure of Zipcodes with their respective lat/long coordinates. 
    //  TODO : override the multiplication operator to generate a new state only containing unique city names contained in both states.
    public class State
    {
        public State(string stateName)
        {
            // read the file and populate the state with zipcodes
        }

        // public static State operator *(State state1, State state2)
        // {
        //     // return a new state with unique city names
        // }
    }

    // Each Zipcode Belongs to a State, has a list of cities, which have a matrix array of lat/long coordinates.
    //  TODO : override the ToString method to display the city name and its coordinates.
    public class Zipcode 
    {
        
    }
}