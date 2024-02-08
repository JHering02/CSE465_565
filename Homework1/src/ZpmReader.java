import java.nio.file.Files;
import java.nio.file.Paths;
import java.io.IOException;
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
        // Fetch all lines from the file into a list and store it as an instance
        // variable
        this.lineList = Files.readAllLines(Paths.get(fileName));
    }

    private void handleFor() {
        // Handle a for loop in zpm
    }

    private void handlePrint(String outString) {
        // Handle a print statement in zpm
        String output = outString.contains("\"") ? outString.replace("\"", "") : 
        outString + "=" + variables.get(outString).toString();
        System.out.println(output);
    }

    // Handle a simple assignment operator in zpm
    private void assign(String varName, String opr, String value) throws Exception {
        if (opr.equals("=")) {
            Object newVal = variables.containsKey(value) ? variables.get(value) :  value;
            newVal = !value.contains("\"") ? (int) Integer.parseInt(value) : (String) newVal.toString().replace("\"", "");
            variables.put(varName, newVal);
        } else if (variables.containsKey(varName) && variables.get(varName) instanceof Integer) {
            int val = (int) variables.get(varName);
            int newVal = variables.containsKey(value) ? (int) variables.get(value) :  Integer.parseInt(value);
            if (opr.equals("+=")) {
                variables.put(varName, val + newVal);
            } else if (opr.equals("-=")) {
                variables.put(varName, val - newVal);
            } else if (opr.equals("*=")) {
                variables.put(varName, val * newVal);
            }
        } else if (variables.containsKey(varName) && variables.get(varName) instanceof String && opr.equals("+=")) {
            // Perform string concatenation if the variable is in the list and is a string
            String val = (String) variables.get(varName);
            variables.put(varName, val + value);
        } else {
            throw new Exception("Bad Syntax: " + varName + opr + value);
        }
    }

    private void interpret(String[] dat) throws Exception {
        int curr = 0;
        while (!dat[curr].equals(";")) {
            // check if the current token is a variable (regex A-Z only)
            if (dat[curr].matches("[A-Z]+") && dat[curr + 1].contains("=")) {
                // check if the next token is an assignment operator
                assign(dat[curr], dat[curr + 1], dat[curr + 2]);
                curr += 3;
            } else if (dat[curr].equals("FOR")) {
                handleFor();
            } else if (dat[curr].equals("PRINT")) {
                handlePrint(dat[curr + 1]);
            } else {
                throw new Exception("Invalid token: " + dat[curr]);
            }
        }
    }

    // Method that reads the zpm file and interprets the code
    // Use regex to follow specific zpm format
    public void readZPM() {
        for (String line : lineList) {
            try {
                interpret(line.split(" "));
            } catch (Exception e) {
                System.out.println("RUNTIME ERROR: line " + (lineList.indexOf(line) + 1));
                System.exit(1);
            }
        }
    }
}
