using System;

namespace ExceptionHandlingDemo
{
    // Custom exception class
    public class NegativeNumberException : Exception
    {
        public NegativeNumberException() : base("Negative numbers are not allowed.")
        {
        }
        
        public NegativeNumberException(string message) : base(message)
        {
        }
        
        public NegativeNumberException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
        
        // Additional property for custom exception
        public int InvalidValue { get; set; }
    }
    
    // Another custom exception
    public class TooLargeValueException : Exception
    {
        public int MaxAllowed { get; }
        public int ProvidedValue { get; }
        
        public TooLargeValueException(int maxAllowed, int providedValue)
            : base($"Value {providedValue} exceeds maximum allowed {maxAllowed}")
        {
            MaxAllowed = maxAllowed;
            ProvidedValue = providedValue;
        }
    }
    
    public class ExceptionHandlingDemo
    {
        public void Demo()
        {
            Console.WriteLine("=== C# Exception Handling Demo ===\n");
            
            // 1. Basic try-catch
            Console.WriteLine("1. Basic try-catch block:");
            try
            {
                int result = DivideNumbers(10, 0);
                Console.WriteLine($"Result: {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Caught DivideByZeroException: {ex.Message}");
            }
            
            // 2. Multiple catch blocks
            Console.WriteLine("\n2. Multiple catch blocks:");
            try
            {
                ProcessNumber(-5);
            }
            catch (NegativeNumberException ex)
            {
                Console.WriteLine($"Caught NegativeNumberException: {ex.Message}");
                Console.WriteLine($"Invalid value was: {ex.InvalidValue}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Caught ArgumentException: {ex.Message}");
            }
            catch (Exception ex) // General catch-all
            {
                Console.WriteLine($"Caught general Exception: {ex.Message}");
            }
            
            // 3. Finally block
            Console.WriteLine("\n3. try-catch-finally block:");
            System.IO.StreamReader reader = null;
            try
            {
                Console.WriteLine("Attempting to read a file...");
                reader = new System.IO.StreamReader("nonexistentfile.txt");
                string content = reader.ReadToEnd();
                Console.WriteLine($"File content: {content}");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.FileName}");
            }
            finally
            {
                Console.WriteLine("Finally block executed.");
                if (reader != null)
                {
                    reader.Close();
                    Console.WriteLine("StreamReader closed in finally block.");
                }
            }
            
            // 4. Using existing MathOperations class that throws exceptions
            Console.WriteLine("\n4. Handling exceptions from existing MathOperations class:");
            MathOperations mathOps = new MathOperations();
            try
            {
                double result = mathOps.Divide(10, 0);
                Console.WriteLine($"Division result: {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Caught DivideByZeroException from MathOperations: {ex.Message}");
            }
            
            // 5. Nested try-catch
            Console.WriteLine("\n5. Nested try-catch blocks:");
            try
            {
                Console.WriteLine("Outer try block");
                try
                {
                    int[] numbers = { 1, 2, 3 };
                    Console.WriteLine($"Accessing element at index 5: {numbers[5]}");
                }
                catch (IndexOutOfRangeException innerEx)
                {
                    Console.WriteLine($"Inner catch: {innerEx.Message}");
                    throw new InvalidOperationException("Array access failed", innerEx);
                }
            }
            catch (InvalidOperationException outerEx)
            {
                Console.WriteLine($"Outer catch: {outerEx.Message}");
                Console.WriteLine($"Inner exception: {outerEx.InnerException?.Message}");
            }
            
            // 6. Exception properties demonstration
            Console.WriteLine("\n6. Exception properties:");
            try
            {
                ValidateAge(150);
            }
            catch (TooLargeValueException ex)
            {
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Max Allowed: {ex.MaxAllowed}");
                Console.WriteLine($"Provided Value: {ex.ProvidedValue}");
            }
            
            // 7. Using when clause for conditional catching
            Console.WriteLine("\n7. Conditional catching with 'when' clause:");
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (i == 0)
                        throw new ArgumentException("Argument error");
                    else if (i == 1)
                        throw new InvalidOperationException("Operation error with code 100", new Exception("Inner error"));
                    else
                        throw new InvalidOperationException("Operation error with code 200");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("code 100"))
                {
                    Console.WriteLine($"Caught InvalidOperationException with code 100: {ex.Message}");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("code 200"))
                {
                    Console.WriteLine($"Caught InvalidOperationException with code 200: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Caught general exception: {ex.GetType().Name} - {ex.Message}");
                }
            }
            
            // 8. Demonstrating exception propagation
            Console.WriteLine("\n8. Exception propagation through call stack:");
            try
            {
                MethodA();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught at top level: {ex.Message}");
                Console.WriteLine($"Full stack trace:\n{ex.StackTrace}");
            }
            
            // 9. Using try-catch with out parameters
            Console.WriteLine("\n9. Exception handling with out parameters:");
            if (TryParsePositiveNumber("abc", out int parsedValue))
            {
                Console.WriteLine($"Parsed value: {parsedValue}");
            }
            else
            {
                Console.WriteLine("Failed to parse positive number.");
            }
            
            if (TryParsePositiveNumber("-10", out parsedValue))
            {
                Console.WriteLine($"Parsed value: {parsedValue}");
            }
            else
            {
                Console.WriteLine("Failed to parse positive number.");
            }
            
            if (TryParsePositiveNumber("42", out parsedValue))
            {
                Console.WriteLine($"Successfully parsed: {parsedValue}");
            }
            
            // 10. Best practices demonstration
            Console.WriteLine("\n10. Exception handling best practices:");
            Console.WriteLine("- Use specific exception types when possible");
            Console.WriteLine("- Don't catch exceptions you can't handle");
            Console.WriteLine("- Clean up resources in finally blocks or use using statements");
            Console.WriteLine("- Provide meaningful error messages");
            Console.WriteLine("- Use custom exceptions for domain-specific errors");
            
            Console.WriteLine("\n=== Exception Handling Demo Complete ===");
        }
        
        // Helper methods for demonstration
        
        private int DivideNumbers(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return a / b;
        }
        
        private void ProcessNumber(int number)
        {
            if (number < 0)
            {
                var ex = new NegativeNumberException();
                ex.InvalidValue = number;
                throw ex;
            }
            
            if (number == 0)
                throw new ArgumentException("Zero is not allowed.");
                
            Console.WriteLine($"Processing number: {number}");
        }
        
        private void ValidateAge(int age)
        {
            const int MAX_AGE = 120;
            if (age > MAX_AGE)
                throw new TooLargeValueException(MAX_AGE, age);
                
            Console.WriteLine($"Age {age} is valid.");
        }
        
        private void MethodA()
        {
            Console.WriteLine("MethodA called");
            MethodB();
        }
        
        private void MethodB()
        {
            Console.WriteLine("MethodB called");
            MethodC();
        }
        
        private void MethodC()
        {
            Console.WriteLine("MethodC called - throwing exception");
            throw new ApplicationException("Error originated in MethodC");
        }
        
        private bool TryParsePositiveNumber(string input, out int result)
        {
            result = 0;
            try
            {
                result = int.Parse(input);
                if (result <= 0)
                    throw new NegativeNumberException("Number must be positive.");
                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine($"FormatException: '{input}' is not a valid integer.");
                return false;
            }
            catch (NegativeNumberException ex)
            {
                Console.WriteLine($"NegativeNumberException: {ex.Message}");
                return false;
            }
            catch (OverflowException)
            {
                Console.WriteLine($"OverflowException: '{input}' is too large or too small.");
                return false;
            }
        }
        
        // Additional demonstration: Using statement for automatic disposal
        public void DemonstrateUsingStatement()
        {
            Console.WriteLine("\nDemonstrating 'using' statement for automatic disposal:");
            
            try
            {
                using (var resource = new DisposableResource())
                {
                    resource.DoSomething();
                    throw new InvalidOperationException("Error while using resource");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
                Console.WriteLine("Resource was automatically disposed even with exception.");
            }
        }
    }
    
    // Helper class for demonstrating disposable pattern
    public class DisposableResource : IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("DisposableResource is doing something...");
        }
        
        public void Dispose()
        {
            Console.WriteLine("DisposableResource is being disposed.");
        }
    }
}