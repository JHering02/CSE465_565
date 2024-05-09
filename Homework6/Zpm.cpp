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
  using varPtr =
      std::variant<std::unique_ptr<std::string>, std::unique_ptr<int>>;
  std::unordered_map<std::string, varPtr> variables;

  void zpmLoop() {}

  void zpmMath() {}

  void zpmEquals() {
    std::smatch match;
    if (std::regex_search(
            zpmLine, match,
            std::regex(
                R"(([a-zA-Z]+\d*)\s*=\s*((\"[^\"]*\")|(-?\d+)|([a-zA-Z]+\d*))\s*;)"))) {
      std::string varName = match[1];
      std::string value = match[2];
      value.erase(std::remove(value.begin(), value.end(), ' '), value.end());
      if (value.front() != '"' && value.back() != '"' &&
          variables.find(value) != variables.end()) {
        if (std::holds_alternative<std::unique_ptr<std::string>>(
                variables[value])) {
          value = *std::get<std::unique_ptr<std::string>>(variables[value]);
          variables[varName] = std::make_unique<std::string>(value);
          return;
        } else {
          value =
              std::to_string(*std::get<std::unique_ptr<int>>(variables[value]));
        }
      }
      if (std::isdigit(value[0]) || (value[0] == '-' && isdigit(value[1]))) {
        variables[varName] = std::make_unique<int>(std::stoi(value));
      } else if ((value.front() == '"' && value.back() == '"')) {
        variables[varName] = std::make_unique<std::string>(value);
      } else {
        std::cerr << "Runtime Error: Variable " << value << " does not exist."
                  << std::endl;
      }
    } else {
      std::cerr << "Runtime Error: Bad Syntax on line " << zpmLine;
    }
  }

  void zpmPrint() {}

public:
  ZpmInterpreter(std::string zpmPath) : zpmContent(zpmPath) {
    zpmFile.open(zpmPath);
    if (!zpmFile.good()) {
      std::cout << "Unable to open " << zpmPath << ". Aborting.\n";
    }
  }
  ~ZpmInterpreter() { zpmFile.close(); }

  void readZpm() {
    while (std::getline(zpmFile, zpmLine)) {
      if (std::regex_match(zpmLine, std::regex(R"(^FOR.*ENDFOR$)"))) {
        // zpmLoop(zpmLine);
      } else if (std::regex_match(zpmLine, std::regex(R"(.*\s+=\s+.*)"))) {
        zpmEquals();
      } else if (std::regex_match(zpmLine, std::regex(R"(^PRINT\s)"))) {
        zpmPrint();
      }
        // if (std::regex_match(
        //         zpmLine,
        //         std::regex(
        //             R"((([a-z|A-Z]*\d)|*\d))\s(+=|-=|*=)\s(([a-z|A-Z]*\d)|*\d))")))
        //   zpmMath(zpmLine);
        continue;
      } else {
        continue;
      }
    }
  }
};

int main(int argc, char *argv[]) {
  if (argc > 1) {
    ZpmInterpreter zpmProg(argv[1]);
    zpmProg.readZpm();
    return 0; // Exit without error
  }
  std::cerr << "Incorrect number of arguments" << std::endl;
  return 1; // Exit with error
}
