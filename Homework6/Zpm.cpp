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
  int lineCount = 0;
  using varPtr =
      std::variant<std::unique_ptr<std::string>, std::unique_ptr<int>>;
  std::unordered_map<std::string, varPtr> variables;

  void zpmLoop() {
    size_t pos = zpmLine.find("FOR");
    size_t endPos = zpmLine.find("ENDFOR");
    if (pos == std::string::npos || endPos == std::string::npos) {
      std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
      return;
    }
    std::istringstream iss(zpmLine.substr(pos + 3)); // Skip "FOR "
    std::string tempIter;
    int iterations;
    iss >> tempIter;
    if (std::regex_match(tempIter, std::regex(R"(\d+)"))) {
      iterations = std::stoi(tempIter);
    } else if (std::holds_alternative<std::unique_ptr<int>>(
                     variables[tempIter])) {
      iterations = *std::get<std::unique_ptr<int>>(variables[tempIter]);
    } else {
      std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
    }
    pos = zpmLine.find(tempIter);
    iss.clear();
    for (int i = 0; i < iterations; ++i) {
      std::istringstream iss(zpmLine.substr(pos + tempIter.size()));
      std::string statement;
      while (std::getline(iss, statement, ';') && statement.find("ENDFOR") == std::string::npos) {
        readZpmLine(statement += ";");
      }
      iss.clear();
    }
  }

  void zpmMath() {
    std::smatch match;
    if (std::regex_search(
            zpmLine, match,
            std::regex(
                R"(([a-zA-Z]+\d*)\s*(\+=|-=|\*=)\s*((\"[^\"]*\")|(-?\d+)|([a-zA-Z]+\d*))\s*;)"))) {
      std::string varName = match[1];
      std::string operation = match[2];
      std::string value = match[3];
      if (!(variables.find(varName) != variables.end())) {
        std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
        return;
      }
      if (std::holds_alternative<std::unique_ptr<std::string>>(
              variables[varName])) {
        if (value.front() == '"' && value.back() == '"') {
          value = value.substr(1, value.size() - 2);
          *std::get<std::unique_ptr<std::string>>(variables[varName]) += value;
        } else if (std::holds_alternative<std::unique_ptr<std::string>>(
                       variables[value])) {
          *std::get<std::unique_ptr<std::string>>(variables[varName]) +=
              *std::get<std::unique_ptr<std::string>>(variables[value]);
        }
      } else if (std::holds_alternative<std::unique_ptr<int>>(
                     variables[varName])) {
        if (std::holds_alternative<std::unique_ptr<int>>(variables[value])) {
          value =
              std::to_string(*std::get<std::unique_ptr<int>>(variables[value]));
        }
        int &intVar = *std::get<std::unique_ptr<int>>(variables[varName]);
        if (operation == "+=" && std::isdigit(value[0]) ||
            (value[0] == '-' && isdigit(value[1]))) {
          intVar += std::stoi(value);
        } else if (operation == "-=" && std::isdigit(value[0]) ||
                   (value[0] == '-' && isdigit(value[1]))) {
          intVar -= std::stoi(value);
        } else if (operation == "*=" && std::isdigit(value[0]) ||
                   (value[0] == '-' && isdigit(value[1]))) {
          intVar *= std::stoi(value);
        } else {
          std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
        }
      } else {
        std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
      }
    }
  }

  void zpmEquals() {
    std::smatch match;
    if (std::regex_search(
            zpmLine, match,
            std::regex(
                R"(([a-zA-Z]+\d*)\s*=\s*((\"[^\"]*\")|(-?\d+)|([a-zA-Z]+\d*))\s*;)"))) {
      std::string varName = match[1];
      std::string value = match[2];
      if (std::regex_match(value, std::regex(R"(^[^\d"].*[^"]$)")) &&
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
      } else if (value.front() == '"' && value.back() == '"') {
        value = value.substr(1, value.size() - 2);
        variables[varName] = std::make_unique<std::string>(value);
      } else {
        std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
      }
    } else {
      std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
    }
  }

  void zpmPrint() {
    std::smatch match;
    if (std::regex_search(
            zpmLine, match,
            std::regex(R"(PRINT\s+("[^"]*"|[a-zA-Z]+\d*)\s*;)"))) {
      std::string value = match[1];
      if (value.front() != '"' && value.back() != '"' &&
          variables.find(value) != variables.end()) {
        if (std::holds_alternative<std::unique_ptr<std::string>>(
                variables[value])) {
          value += "=\"" +
                   *std::get<std::unique_ptr<std::string>>(variables[value]) +
                   "\"\n";
        } else {
          value += "=" +
                   std::to_string(
                       *std::get<std::unique_ptr<int>>(variables[value])) +
                   "\n";
        }
      } else if (value.front() == '"' && value.back() == '"') {
        value = value.substr(1, value.size() - 2);
      } else {
        std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
      }
      std::cout << value;
    } else {
      std::cerr << "RUNTIME ERROR: line " << lineCount << std::endl;
    }
  }

public:
  ZpmInterpreter(std::string zpmPath) : zpmContent(zpmPath) {
    zpmFile.open(zpmPath);
    if (!zpmFile.good()) {
      std::cout << "Unable to open " << zpmPath << ". Aborting.\n";
    }
  }
  ~ZpmInterpreter() {
    zpmFile.close();
    variables.clear();
  }

  void readZpm() {
    while (std::getline(zpmFile, zpmLine)) {
      lineCount++;
      if (std::regex_match(zpmLine, std::regex(R"(^FOR.*ENDFOR$)"))) {
        zpmLoop();
      } else if (std::regex_match(zpmLine, std::regex(R"(.*\s+=\s+.*)"))) {
        zpmEquals();
      } else if (std::regex_match(zpmLine, std::regex(R"(PRINT\s+.*)"))) {
        zpmPrint();
      } else if (std::regex_match(zpmLine,
                                  std::regex(R"(.*\s+(\+=|-=|\*=)\s+.*)"))) {
        zpmMath();
      }
    }
  }
  void readZpmLine(std::string &zpmLine) {
    if (std::regex_match(zpmLine, std::regex(R"(.*\s+=\s+.*)"))) {
      zpmEquals();
    } else if (std::regex_match(zpmLine, std::regex(R"(PRINT\s+.*)"))) {
      zpmPrint();
    } else if (std::regex_match(zpmLine,
                                std::regex(R"(.*\s(\+=|-=|\*=)\s+.*)"))) {
      zpmMath();
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
