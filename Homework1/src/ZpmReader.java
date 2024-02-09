import java.nio.file.Files;
import java.nio.file.Paths;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
 * James Hering
 * CSE465 - Comparative Programming Languages
 * 4 February 2024
 * 
 * A basic Z+- interpreter which executes zpm code from a file in the command line which reads string/integer variables
 * and can also read loops. The interpreter will also detect runtime errors  
 */
public class ZpmReader {
    // private instance variable containing the filename
    private String fileName;

    // private instance variable containing a list of each line in the file
    private List<String> lineList;

    // private map instance variable that contains all saved variables during
    // runtime
    private Map<String, Object> variables = new HashMap<String, Object>();

    // Basic parameter constructor
    public ZpmReader(String fileName) throws IOException {
        this.fileName = fileName;
        // Store all lines from the file into a list
        this.lineList = Files.readAllLines(Paths.get(fileName));
    }

    private void handleFor(String[] dat, int curr) throws Exception {
        int loopCount = Integer.parseInt(dat[curr + 1]);
        // Create an Array copy without 'FOR', the loop count, and 'ENDFOR'
        String[] forDat = Arrays.copyOfRange(dat, curr + 2, dat.length - 1);
        for (int i = 0; i < loopCount; i++)
            interpret(forDat, 0);
    }

    private void handlePrint(String outString) {
        // Handle a print statement in zpm
        String output = outString.contains("\"") ? outString.replace("\"", "")
                : outString + "=" + variables.get(outString).toString();
        System.out.println(output);
    }

    private void handleOperands(String varName, String opr, String value) {
         // Long and annoying way of handling operands because they can't be parsed
         int val = (int) variables.get(varName);
         int newVal = variables.containsKey(value) ? (int) variables.get(value)
                 : Integer.parseInt(value);
         if (opr.equals("+=")) {
             variables.put(varName, val + newVal);
         } else if (opr.equals("-=")) {
             variables.put(varName, val - newVal);
         } else if (opr.equals("*=")) {
             variables.put(varName, val * newVal);
         }
    }

    private void assign(String varName, String opr, String value) throws Exception {
         // Handle a simple assignment operator in zpm
        if (opr.equals("=")) {
            Object newVal = variables.containsKey(value) ? variables.get(value) : value;
            newVal = !value.contains("\"") ? (int) Integer.parseInt(value)
                    : (String) newVal.toString().replace("\"", "");
            variables.put(varName, newVal);
        } else if (variables.containsKey(varName) && variables.get(varName) instanceof Integer) {
           handleOperands(varName, opr, value);
        } else if (variables.containsKey(varName) &&
                variables.get(varName) instanceof String &&
                opr.equals("+=")) {
            // Perform string concatenation if the variable is in the list and is a string
            String oldVal = (String) variables.get(varName);
            String val = !value.contains("\"") ? (String) variables.get(value)
                    : (String) value.toString().replace("\"", "");
            if (val == null)
                throw new Exception("Bad Syntax: " + varName + opr + value);
            variables.put(varName, oldVal + val);
        } else {
            throw new Exception("Bad Syntax: " + varName + opr + value);
        }
    }

    private void interpret(String[] dat, int curr) throws Exception {
        // Change the loop to deal with multiple semi colons in the line
        while (curr < dat.length) {
            // check if the current token is a variable (regex A-Z only)
            if (dat[curr].matches("[A-Z]+") && dat[curr + 1].contains("=")) {
                // check if the next token is an assignment operator
                assign(dat[curr], dat[curr + 1], dat[curr + 2]);
                curr += 3;
            } else if (dat[curr].equals("FOR")) {
                // Handle a for loop in zpm
                handleFor(dat, curr);
                break;
            } else if (dat[curr].equals("PRINT")) {
                // Handle a print statement in zpm
                handlePrint(dat[curr + 1]);
                curr += 2;
            }
            if (!dat[curr].equals(";")) {
                // Check for semicolons
                throw new Exception("Invalid token: " + dat[curr]);
            }
            curr++;
        }
    }

    // Method that reads the zpm file and interprets the code
    // Use regex to follow specific zpm format
    public void readZPM() {
        for (String line : lineList) {
            try {
                interpret(line.split(" "), 0);
            } catch (Exception e) {
                System.out.println("RUNTIME ERROR: line " + (lineList.indexOf(line) + 1));
                System.exit(1);
            }
        }
    }
}
