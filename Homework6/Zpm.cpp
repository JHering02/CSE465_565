/**
 * This is the main file that's function is to interpret a zpm file extension
 * using pointers.
 *
 * James Hering
 * 7 May 2024
 */

#include <fstream>
#include <iostream>
#include <memory>
#include <regex>
#include <string>
#include <unordered_map>
#include <variant>
class ZpmInterpreter {
private:
  std::string zpmContent, zpmLine;
  std::ifstream zpmFile;

public:
  using varPtr =
      std::variant<std::unique_ptr<std::string>, std::unique_ptr<int>>;
  ZpmInterpreter(std::string zpmPath) : zpmContent(zpmPath) {
    zpmFile.open(zpmPath);
    if (!zpmFile.good()) {
      std::cout << "Unable to open " << zpmPath << ". Aborting.\n";
    }
  }
  ~ZpmInterpreter() { zpmFile.close(); }

  void readZpm() {
    while (std::getline(zpmFile, zpmLine)) {
      std::cout << zpmLine;
    }
  }
};

int main(int argc, char *argv[]) {
  if (argc > 1) {
    ZpmInterpreter zpmProg(argv[1]);
    zpmProg.readZpm();
    return 0; // Exit without error
  }
  std::cerr << "Too many arguments" << std::endl;
  return 1; // Exit with error
}