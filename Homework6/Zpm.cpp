/**
 * This is the main file that's function is to interpret a zpm file extension
 * using pointers.
 *
 * James Hering
 * 7 May 2024
 */

#include <cstring>
#include <exception>
#include <fstream>
#include <iostream>
#include <memory>
#include <regex>
#include <stdexcept>
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
      return;
    }
    pos = zpmLine.find(tempIter);
    iss.clear();
    for (int i = 0; i < iterations; ++i) {
      std::istringstream isstr(zpmLine.substr(pos + tempIter.size()));
      std::string statement;
      while (std::getline(iss, statement, ';') &&
             statement.find("ENDFOR") == std::string::npos) {
        readZpmLine(statement += ";");
      }
      isstr.clear();
    }
  }

  void zpmOperateMath(std::string &value, std::string &operation, int &intVar) {
    if (operation.compare("+=") == 0 && (std::isdigit(value[0]) ||
                              (value[0] == '-' && isdigit(value[1])))) {
      intVar += std::stoi(value);
    } else if (operation.compare("-=") == 0  && (std::isdigit(value[0]) ||
                                     (value[0] == '-' && isdigit(value[1])))) {
      intVar -= std::stoi(value);
    } else if (operation.compare("*=") == 0 && (std::isdigit(value[0]) ||
                                     (value[0] == '-' && isdigit(value[1])))) {
      intVar *= std::stoi(value);
    } else {
      throw std::runtime_error("RUNTIME ERROR: line " +
                               std::to_string(lineCount));
    }
  }

  void zpmMath(std::string &currLine) {
    std::smatch match;
    if (std::regex_search(
            currLine, match,
            std::regex(
                R"(([a-zA-Z]+\d*)\s*(\+=|-=|\*=)\s*((\"[^\"]*\")|(-?\d+)|([a-zA-Z]+\d*))\s*;)"))) {
      std::string varName = match[1];
      std::string operation = match[2];
      std::string value = match[3];
      if (!(variables.find(varName) != variables.end())) {
        throw std::runtime_error("RUNTIME ERROR: line " +
                                 std::to_string(lineCount));
      }
      bool varIsString = std::holds_alternative<std::unique_ptr<std::string>>(
          variables[varName]);
      bool valueIsInt =
          std::holds_alternative<std::unique_ptr<int>>(variables[value]);
      if (varIsString) {
        if (value.front() == '"' && value.back() == '"') {
          value = value.substr(1, value.size() - 2);
          *std::get<std::unique_ptr<std::string>>(variables[varName]) += value;
        } else if (!valueIsInt) {
          *std::get<std::unique_ptr<std::string>>(variables[varName]) +=
              *std::get<std::unique_ptr<std::string>>(variables[value]);
        } else {
          throw std::runtime_error("RUNTIME ERROR: line " +
                                   std::to_string(lineCount));
        }
      } else if (!varIsString) {
        value = (valueIsInt) ? std::to_string(*std::get<std::unique_ptr<int>>(
                                   variables[value]))
                             : value;
        int &intVar = *std::get<std::unique_ptr<int>>(variables[varName]);
        zpmOperateMath(value, operation, intVar);
      } else {
        throw std::runtime_error("RUNTIME ERROR: line " +
                                 std::to_string(lineCount));
      }
    }
  }

  void zpmEquals(std::string &currLine) {
    std::smatch match;
    if (std::regex_search(
            currLine, match,
            std::regex(
                R"(([a-zA-Z]+\d*)\s*=\s*((\"[^\"]*\")|(-?\d+)|([a-zA-Z]+\d*))\s*;)"))) {
      std::string varName = match[1];
      std::string value = match[2];
      bool valueIsString = std::holds_alternative<std::unique_ptr<std::string>>(
          variables[value]);
      if (std::isdigit(value[0]) || (value[0] == '-' && isdigit(value[1]))) {
        variables[varName] = std::make_unique<int>(std::stoi(value));
      } else if (value.front() == '"' && value.back() == '"') {
        value = value.substr(1, value.size() - 2);
        variables[varName] = std::make_unique<std::string>(value);
      } else if (variables.find(value) != variables.end()) {
        if (valueIsString) {
          variables[varName] = std::make_unique<std::string>(
              *std::get<std::unique_ptr<std::string>>(variables[value]));
        } else {
          variables[varName] = std::make_unique<int>(
              *std::get<std::unique_ptr<int>>(variables[value]));
        }
      } else {
        throw std::runtime_error("RUNTIME ERROR: line " +
                                 std::to_string(lineCount));
      }
    } else {
      throw std::runtime_error("RUNTIME ERROR: line " +
                               std::to_string(lineCount));
    }
  }

  void zpmPrint(std::string &currLine) {
    std::smatch match;
    if (std::regex_search(
            currLine, match,
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
        throw std::runtime_error("RUNTIME ERROR: line " +
                                 std::to_string(lineCount));
      }
      std::cout << value;
    } else {
      throw std::runtime_error("RUNTIME ERROR: line " +
                               std::to_string(lineCount));
    }
  }

public:
  ZpmInterpreter(std::string zpmPath) : zpmContent(zpmPath) {
    zpmFile.open(zpmPath);
    if (!zpmFile.good()) {
      throw std::runtime_error("Unable to open " + zpmPath + ". Aborting.\n");
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
        zpmEquals(zpmLine);
      } else if (std::regex_match(zpmLine, std::regex(R"(PRINT\s+.*)"))) {
        zpmPrint(zpmLine);
      } else if (std::regex_match(zpmLine,
                                  std::regex(R"(.*\s+(\+=|-=|\*=)\s+.*)"))) {
        zpmMath(zpmLine);
      }
    }
  }
  void readZpmLine(std::string &currLine) {
    if (std::regex_match(currLine, std::regex(R"(.*\s+=\s+.*)"))) {
      zpmEquals(currLine);
    } else if (std::regex_match(currLine, std::regex(R"(\s*PRINT\s+.*)"))) {
      zpmPrint(currLine);
    } else if (std::regex_match(currLine,
                                std::regex(R"(.*\s(\+=|-=|\*=)\s+.*)"))) {
      zpmMath(currLine);
    }
  }
};

int main(int argc, char *argv[]) {
  if (argc > 1) {
    try {
      ZpmInterpreter zpmProg(argv[1]);
      zpmProg.readZpm();
      return 0; // Exit without error
    } catch (const std::exception &e) {
      std::cerr << e.what() << std::endl;
    }
  } else {
    std::cerr << "Incorrect number of arguments" << std::endl;
  }
  return 1; // Exit with error
}
