import java.nio.file.Files;
import java.nio.file.Paths;
import java.io.IOException;
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
    private Map<String, Object> variables;

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

    private void handlePrint() {
        // Handle a print statement in zpm
    }

    // Handle a simple assignment operator in zpm
    private void assign(String varName, String opr, String value) {
    }

    private void interpret(String[] dat) {
        int curr = 0;
        do {
            // check if the current token is a variable (regex A-Z only)
            if (dat[curr].matches("[A-Z]+") && dat[curr + 1].contains("=")) {
                // check if the next token is an assignment operator
                assign(dat[curr], dat[curr + 1], dat[curr + 2]);
            } else if (dat[curr].equals("FOR")) {
                handleFor();
            } else if (dat[curr].equals("PRINT")) {
                handlePrint();
            }
            curr++;
        } while (dat[curr] != ";");
    }

    // Method that reads the zpm file and interprets the code
    // Use regex to follow specific zpm format
    public void readZPM() {
        for (String line : lineList) {
            try {
                interpret(line.split(" "));
            } catch (Exception e) {
                System.out.println("RUNTIME ERROR: line " + lineList.indexOf(line));
                System.exit(1);
            }
        }
    }
}
