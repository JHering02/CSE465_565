

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
#   List Comprehension - TODO
#   Data Structures (Dictionary, set, etc)
#   Variable Positional Argument - TODO
#   Yield - TODO


class LatLon:
    outFileName = "./LatLon.txt"
    latFile = "./zips.txt"

    def __init__(self, inputReader):
        self.inputReader = inputReader

    def outText(self, zipList):
        outFile = open(latFile)

        outFile.close()


class CommonCities:
    outFilename = "./CityStates.txt"
    statefile = "./states.txt"

    def __init__(self, inputReader):
        self.inputReader = inputReader


class CityStates:
    outFilename = "./CommonCityNames.txt"
    citFile = "./cities.txt"

    def __init__(self, inputReader):
        self.inputReader = inputReader
