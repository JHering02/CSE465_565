

#    Homework 5 Operations

#    James Hering

#    18 April 2024

#     This file contains python classes for processing the city data and writing the output files.
#     There are three main operations being done here:

#     1. LatLon - Writes the latitude and longitude of each given zipcode in zips.txt to a file by using the overidden == operator of the Zipcode class,
#                 and finding values in a list of zipcodes from 'zips.txt'.

#     2. CommonCityNames - Writes only common city names between all states in a 'states.txt' file alphabetically using sorted HashSet and writes to a file.

#     3. CityStates - Writes all the states that contain a cityname given by 'cities.txt' using the Any() part of a list, and sorts them to display
#                     each state abbrebiation alphabetically separated by spaces to a file.

#     List of features implemented throughout the code:

#   Lambda - TODO
#   Map - TODO
#   Filter - TODO
#   List Comprehension - Contained within the readZips method in "hw5.py". We use the readlines() method to transfer all strings to a list, and map the zipcodes
#   Data Structures (Dictionary, set, etc) - Contained within the readZips method in "hw5.py". Uses nested dictionaries, and a list of tuples for each city.
#   Variable Positional Argument - TODO
#   Yield - TODO


class LatLon:
    latFile = "./zips.txt"

    def __init__(self, outFileName):
        self.outFileName = outFileName

    def outText(self, statesDict):
        inFile = open(self.latFile)
        outFile = open(self.outFileName, "w")
        outString = list()
        zips = inFile.readlines()
        inFile.close()
        for zip in zips:
            zip = zip.split()[0]
            for state, foundzip in statesDict.items():
                if zip in foundzip:
                    outString.append("{lat}, {lon}".format(
                        lat=foundzip[zip][0][1], lon=foundzip[zip][0][2]))
                    outString.append('\n')
        outString.pop()
        outFile.writelines(outString)
        outFile.close()


class CommonCities:
    stateFile = "./states.txt"

    def __init__(self, outFileName):
        self.outFileName = outFileName

    def outText(self, statesDict):
        inFile = open(self.stateFile)
        outFile = open(self.outFileName, "a")
        states = inFile.readlines()
        state1 = states.pop(0).split()[0]
        for zip in zips:
            zip = zip.split()[0]
            for state, foundzip in statesDict.items():
                if zip in foundzip:
                    outFile.write("{lat}, {lon}\n".format(
                        lat=foundzip[zip][0][1], lon=foundzip[zip][0][2]))
        inFile.close()
        outFile.close()


class CityStates:
    outFilename = "./CommonCityNames.txt"
    citFile = "./cities.txt"

    def __init__(self, inputReader):
        self.inputReader = inputReader
