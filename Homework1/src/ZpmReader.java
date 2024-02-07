import java.util.Scanner;
import java.io.File;

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
    
    // Basic parameter constructor
    public ZpmReader(String fileName) {
        this.fileName = fileName;
    }

    // Use regex to follow specific zpm format
    public void read() {
        System.out.println("Reading file: " + fileName);
    }
}
