import java.io.FileNotFoundException;

/*
 * James Hering
 * CSE465 - Comparative Programming Languages
 * 4 February 2024
 * 
 * A basic Z+- interpreter which executes zpm code from a file in the command line which reads string/integer variables
 * and can also read loops. The interpreter will also detect runtime errors  
 */
public class Zpm {
    public static void main(String[] args)  throws Exception, FileNotFoundException {
        try {
            // Start by making sure there is an argument in the command line
            if (args.length == 0) {
                throw new IllegalArgumentException("No file name provided");
            } // Check if the file is a zpm file
            else if (!args[0].endsWith(".zpm")) {
                throw new IllegalArgumentException("File must be a .zpm file");
            } else {
                // Create a new ZpmReader object
                ZpmReader zpm = new ZpmReader(args[0]);
                // Read the file
                zpm.read();
            }
        } catch(Exception e) {
            System.out.println(e.getMessage());
            System.exit(1);
        }
    }
}
