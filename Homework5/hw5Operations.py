

#    Homework 5 Operations

#    James Hering

#    18 April 2024

#     This file contains python classes for processing the city data and writing the output files.
#     There are three main operations being done here:

#     1. LatLon - Writes the latitude and longitude of each given zipcode in zips.txt to a file using the LatLon class

#     2. CommonCityNames - Writes only common city names between all states in a 'states.txt' file alphabetically using a set and writes to a file.

#     3. CityStates - Writes all the states that contain a cityname given by 'cities.txt' using the Any() part of a list, and sorts them to display
#                     each state abbrebiation alphabetically separated by spaces to a file.

#     List of features implemented throughout the code:

#   Lambda - used in CommonCities method with filter to check if 'x' key is in the states list
#   Map - used with the extract_tuples method, to map a list of state keys to each city name contained within a list of tuples, and to sort the Common city set
#   Filter - Used in the CommonCities method to only read the dictionary keys we are interested in for the dictionary O(1)
#   List Comprehension - Contained within the readZips method in "hw5.py". We use the readlines() method to transfer all strings to a list, and map the zipcodes
#   Data Structures (Dictionary, set, etc) - Contained within the readZips method in "hw5.py". Uses nested dictionaries, and a list of tuples for each city.
#   Variable Positional Argument - TODO
#   Yield - used in CommonCities extract_tuples method to yield each city name in the list of tuples


class LatLon:
    latFile = "./zips.txt"

    def __init__(self, outFileName):
        self.outFileName = outFileName

    def outText(self, statesDict):
        inFile = open(self.latFile)
        outFile = open(self.outFileName, "w")
        outString = list()
        for zip in iter(lambda: inFile.readline(), ''):
            zip = zip.split()[0]
            for state, zipcodes in statesDict.items():
                if zip in zipcodes.keys():
                    outString.append("{lat}, {lon}".format(
                        lat=zipcodes[zip][0][1], lon=zipcodes[zip][0][2]))
                    outString.append('\n')
        inFile.close()
        outString.pop()
        outFile.writelines(outString)
        outFile.close()


class CommonCities:
    stateFile = "./states.txt"

    def __init__(self, outFileName):
        self.outFileName = outFileName

    def outText(self, statesDict):
        def extract_tuples(state_key):
            stateZips = statesDict.get(state_key, {})
            for tuples in stateZips.values():
                for tup in tuples:
                    yield tup[0]
        inFile = open(self.stateFile)
        outFile = open(self.outFileName, "w")
        states = list()
        for state in iter(lambda: inFile.readline(), '\n'):
            states.append(state.split()[0])
        inFile.close()
        stateTuples = map(extract_tuples, filter(
            lambda x: x in states, statesDict.keys()))
        commonCities = None
        for cities in stateTuples:
            if commonCities is None:
                commonCities = set(cities)
            else:
                commonCities &= set(cities)
        sortedCommon = map(lambda x: x + '\n', sorted(commonCities))
        outFile.writelines(sortedCommon)
        outFile.close()


class CityStates:
    outFilename = "./CityStates.txt"
    citFile = "./cities.txt"

    def __init__(self, outFileName):
        self.outFileName = outFileName

    def outText(self, statesDict):
        inFile = open(self.stateFile)
        outFile = open(self.outFileName, "w")
