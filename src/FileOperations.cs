using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FileOperationsDemo
{
    public class FileOperations
    {
        public void Demo()
        {
            Console.WriteLine("=== C# File I/O Operations Demo ===\n");
            
            // Define some test directories and files
            string baseDir = "FileDemo";
            string textFilePath = Path.Combine(baseDir, "sample.txt");
            string csvFilePath = Path.Combine(baseDir, "data.csv");
            string logFilePath = Path.Combine(baseDir, "app.log");
            string backupDir = Path.Combine(baseDir, "Backup");
            
            try
            {
                // 1. Create directory and check existence
                Console.WriteLine("1. Directory Operations:");
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                    Console.WriteLine($"Created directory: {baseDir}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {baseDir}");
                }
                
                // Create backup directory
                Directory.CreateDirectory(backupDir);
                Console.WriteLine($"Created backup directory: {backupDir}");
                
                // 2. Write text to a file using StreamWriter
                Console.WriteLine("\n2. Writing to a text file:");
                using (StreamWriter writer = new StreamWriter(textFilePath))
                {
                    writer.WriteLine("Hello, File I/O World!");
                    writer.WriteLine("This is a sample text file.");
                    //writer.WriteLine($"Created on: {DateTime.Now}");
                    writer.WriteLine("Line 4: More content here.");
                    writer.WriteLine("Line 5: The quick brown fox jumps over the lazy dog.");
                    Console.WriteLine($"Written content to: {textFilePath}");
                }
                
                // 3. Read text from a file using StreamReader
                Console.WriteLine("\n3. Reading from a text file:");
                using (StreamReader reader = new StreamReader(textFilePath))
                {
                    Console.WriteLine($"Content of {textFilePath}:");
                    Console.WriteLine("--- Start of file ---");
                    string line;
                    int lineNumber = 1;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine($"{lineNumber}: {line}");
                        lineNumber++;
                    }
                    Console.WriteLine("--- End of file ---");
                }
                
                // 4. Append text to an existing file
                Console.WriteLine("\n4. Appending to a text file:");
                using (StreamWriter writer = new StreamWriter(textFilePath, append: true))
                {
                    writer.WriteLine("\n[Appended Content]");
                    writer.WriteLine("This line was appended to the file.");
                    //writer.WriteLine($"Appended at: {DateTime.Now:HH:mm:ss}");
                    Console.WriteLine("Appended 3 lines to the file.");
                }
                
                // Read the updated file
                Console.WriteLine("\nUpdated file content:");
                string[] allLines = File.ReadAllLines(textFilePath);
                Console.WriteLine($"Total lines in file: {allLines.Length}");
                
                // 5. Working with CSV file
                Console.WriteLine("\n5. Working with CSV file:");
                WriteCsvFile(csvFilePath);
                ReadCsvFile(csvFilePath);
                
                // 6. File class static methods
                Console.WriteLine("\n6. File class static methods:");
                
                // Check if file exists
                bool fileExists = File.Exists(textFilePath);
                Console.WriteLine($"File exists: {fileExists}");
                
                // Get file information
                FileInfo fileInfo = new FileInfo(textFilePath);
                Console.WriteLine($"File size: {fileInfo.Length} bytes");
                Console.WriteLine($"File creation time: {fileInfo.CreationTime}");
                Console.WriteLine($"File last write time: {fileInfo.LastWriteTime}");
                
                // Copy file
                string copiedFilePath = Path.Combine(backupDir, "sample_backup.txt");
                File.Copy(textFilePath, copiedFilePath, overwrite: true);
                Console.WriteLine($"File copied to: {copiedFilePath}");
                
                // 7. Directory class operations
                Console.WriteLine("\n7. Directory class operations:");
                
                string[] files = Directory.GetFiles(baseDir);
                Console.WriteLine($"Files in {baseDir}:");
                foreach (string file in files)
                {
                    Console.WriteLine($"  - {Path.GetFileName(file)}");
                }
                
                string[] directories = Directory.GetDirectories(baseDir);
                Console.WriteLine($"\nSubdirectories in {baseDir}:");
                foreach (string dir in directories)
                {
                    Console.WriteLine($"  - {Path.GetFileName(dir)}");
                }
                
                // 8. Path class operations
                Console.WriteLine("\n8. Path class operations:");
                Console.WriteLine($"Directory name: {Path.GetDirectoryName(textFilePath)}");
                Console.WriteLine($"File name: {Path.GetFileName(textFilePath)}");
                Console.WriteLine($"File name without extension: {Path.GetFileNameWithoutExtension(textFilePath)}");
                Console.WriteLine($"Extension: {Path.GetExtension(textFilePath)}");
                Console.WriteLine($"Full path: {Path.GetFullPath(textFilePath)}");
                Console.WriteLine($"Temporary file path: {Path.GetTempFileName()}");
                Console.WriteLine($"Temporary directory: {Path.GetTempPath()}");
                
                // 9. Reading all text at once
                Console.WriteLine("\n9. Reading all text at once:");
                string allText = File.ReadAllText(textFilePath);
                Console.WriteLine($"First 100 characters:\n{allText.Substring(0, Math.Min(100, allText.Length))}...");
                
                // 10. Writing all lines at once
                Console.WriteLine("\n10. Writing all lines at once:");
                string[] linesToWrite = {
                    "Line 1: Written with WriteAllLines",
                    "Line 2: Another line",
                    "Line 3: Third line",
                    "Line 4: Final line"
                };
                string multiLineFile = Path.Combine(baseDir, "multiline.txt");
                File.WriteAllLines(multiLineFile, linesToWrite);
                Console.WriteLine($"Written {linesToWrite.Length} lines to: {multiLineFile}");
                
                // 11. Binary file operations (simple example)
                Console.WriteLine("\n11. Binary file operations:");
                string binaryFile = Path.Combine(baseDir, "data.bin");
                using (FileStream fs = new FileStream(binaryFile, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(42); // int
                    writer.Write(3.14159); // double
                    writer.Write(true); // bool
                    writer.Write("Hello Binary"); // string
                    Console.WriteLine($"Written binary data to: {binaryFile}");
                }
                
                using (FileStream fs = new FileStream(binaryFile, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int intValue = reader.ReadInt32();
                    double doubleValue = reader.ReadDouble();
                    bool boolValue = reader.ReadBoolean();
                    string stringValue = reader.ReadString();
                    
                    Console.WriteLine($"Read from binary file:");
                    Console.WriteLine($"  int: {intValue}");
                    Console.WriteLine($"  double: {doubleValue}");
                    Console.WriteLine($"  bool: {boolValue}");
                    Console.WriteLine($"  string: {stringValue}");
                }
                
                // 12. File logging example
                Console.WriteLine("\n12. File logging example:");
                LogMessage(logFilePath, "Application started");
                LogMessage(logFilePath, "Processing data...");
                LogMessage(logFilePath, "Data processing completed");
                LogMessage(logFilePath, "Application shutting down");
                
                Console.WriteLine("\nLog file content:");
                using (StreamReader reader = new StreamReader(logFilePath))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
                
                // 13. Error handling in file operations
                Console.WriteLine("\n13. Error handling in file operations:");
                TryProblematicFileOperation();
                
                // 14. Cleanup (optional - comment out to keep files)
                Console.WriteLine("\n14. Cleanup:");
                Console.WriteLine("Files created during demo:");
                DisplayDirectoryTree(baseDir, 0);
                
                Console.WriteLine("\nTo keep the demo files, skip the cleanup.");
                Console.WriteLine("To delete them, uncomment the cleanup code in the demo.");
                
                // Uncomment to clean up
                // CleanupDemoFiles(baseDir);
                // Console.WriteLine("Demo files cleaned up.");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file operations: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\n=== File I/O Demo Complete ===");
        }
        
        private void WriteCsvFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Name,Age,City,Salary");
                
                // Write data rows
                writer.WriteLine("John Doe,30,New York,75000");
                writer.WriteLine("Jane Smith,28,Los Angeles,68000");
                writer.WriteLine("Bob Johnson,35,Chicago,82000");
                writer.WriteLine("Alice Brown,26,San Francisco,71000");
                
                Console.WriteLine($"Created CSV file: {filePath}");
            }
        }
        
        private void ReadCsvFile(string filePath)
        {
            Console.WriteLine($"\nReading CSV file: {filePath}");
            using (StreamReader reader = new StreamReader(filePath))
            {
                string header = reader.ReadLine();
                Console.WriteLine($"Header: {header}");
                
                Console.WriteLine("Data rows:");
                string line;
                int rowCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    rowCount++;
                    string[] columns = line.Split(',');
                    Console.WriteLine($"  Row {rowCount}: Name={columns[0]}, Age={columns[1]}, City={columns[2]}, Salary={columns[3]}");
                }
                Console.WriteLine($"Total rows: {rowCount}");
            }
        }
        
        private void LogMessage(string logFilePath, string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
        
        private void TryProblematicFileOperation()
        {
            try
            {
                // Try to read a non-existent file
                string nonExistentFile = "nonexistentfile.txt";
                string content = File.ReadAllText(nonExistentFile);
                Console.WriteLine($"Content: {content}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            
            try
            {
                // Try to write to a read-only file (if it exists)
                string readOnlyFile = "readonly.txt";
                if (!File.Exists(readOnlyFile))
                {
                    File.WriteAllText(readOnlyFile, "Test content");
                    File.SetAttributes(readOnlyFile, FileAttributes.ReadOnly);
                    Console.WriteLine($"Created and set as read-only: {readOnlyFile}");
                }
                
                // This should throw UnauthorizedAccessException
                File.WriteAllText(readOnlyFile, "New content");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Expected error - UnauthorizedAccess: {ex.Message}");
                
                // Clean up the read-only file
                string readOnlyFile = "readonly.txt";
                if (File.Exists(readOnlyFile))
                {
                    File.SetAttributes(readOnlyFile, FileAttributes.Normal);
                    File.Delete(readOnlyFile);
                    Console.WriteLine("Cleaned up read-only test file.");
                }
            }
        }
        
        private void DisplayDirectoryTree(string path, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 2);
            
            // Display current directory
            Console.WriteLine($"{indent}[{Path.GetFileName(path) ?? path}]");
            
            try
            {
                // Display files in current directory
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    Console.WriteLine($"{indent}  {Path.GetFileName(file)} ({fi.Length} bytes)");
                }
                
                // Recursively display subdirectories
                string[] directories = Directory.GetDirectories(path);
                foreach (string dir in directories)
                {
                    DisplayDirectoryTree(dir, indentLevel + 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"{indent}  [Access Denied]");
            }
        }
        
        private void CleanupDemoFiles(string baseDir)
        {
            if (Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, recursive: true);
                Console.WriteLine($"Deleted directory: {baseDir}");
            }
        }
        
        // Additional useful methods for file operations
        
        public long GetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return -1; // File doesn't exist
        }
        
        public List<string> SearchFiles(string directory, string searchPattern)
        {
            List<string> foundFiles = new List<string>();
            
            if (Directory.Exists(directory))
            {
                try
                {
                    foundFiles.AddRange(Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly));
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Access denied to directory: {directory}");
                }
            }
            
            return foundFiles;
        }
        
        public void CreateFileWithContent(string filePath, string content)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, content);
        }
        
        public string ReadFileSafely(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    return $"File not found: {filePath}";
                }
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }
    }
}