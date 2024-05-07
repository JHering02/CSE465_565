/**
 * This is the main file that's function is to interpret a zpm file extension using pointers.
 *
 * James Hering
 * 7 May 2024
 */

#include <iostream>
#include <fstream>
#include <string>
class ZpmReader {
public:
    std::string zpmPath;
    ZpmReader(std::string zpmProg) : zpmPath(zpmProg) {
        
    }
};

int main(int argc, char *argv[]) {
  if (argc > 1) {
    std::ifstream getReq(argv[1]);
    if (!getReq.good()) {
      std::cout << "Unable to open " << argv[1] << ". Aborting.\n";
      return 2; // non-zero exit code to signify error
    }
    std::string content((std::istreambuf_iterator<char>(getReq)),
                        std::istreambuf_iterator<char>());
    return 0; // Exit without error
  }
  std::cerr << "Too many arguments" << std::endl;
  return 1; // Exit with error
}