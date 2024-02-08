
import java.nio.file.Files;
import java.nio.file.Paths;
import java.io.FileNotFoundException;
import java.io.FileReader;

/*
 * James Hering
 * CSE465 - Comparative Programming Languages
 * 4 February 2024
 * 
 * A basic Z+- interpreter which executes zpm code from a file in the command line which reads string/integer variables
 * and can also read loops. The interpreter will also detect runtime errors  
 */
public class ZpmReader {
    private String fileName;

    public ZpmReader(String fileName) {
        this.fileName = fileName;
    }

    // Handle a simple assignment statement in zpm
    private void assign() {

    }

    // Handle compoud assignment statement in zpm
    private void compAssign() {

    }
 
    // Method that reads the zpm file and interprets the code
    public void read() {
       
    }
}
