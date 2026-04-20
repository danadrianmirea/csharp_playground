using System;
using System.Collections.Generic;

namespace DelegateEventDemo
{
    // 1. Custom delegate declarations
    public delegate int BinaryOperation(int a, int b);
    public delegate void NotificationDelegate(string message);
    public delegate bool FilterDelegate(int number);
    
    // 2. Custom EventArgs for events
    public class CalculationEventArgs : EventArgs
    {
        public string Operation { get; }
        public int Operand1 { get; }
        public int Operand2 { get; }
        public int Result { get; }
        
        public CalculationEventArgs(string operation, int operand1, int operand2, int result)
        {
            Operation = operation;
            Operand1 = operand1;
            Operand2 = operand2;
            Result = result;
        }
    }
    
    // 3. A class that raises events
    public class Calculator
    {
        // Event declaration using EventHandler<T>
        public event EventHandler<CalculationEventArgs> CalculationPerformed;
        
        // Method that raises the event
        protected virtual void OnCalculationPerformed(CalculationEventArgs e)
        {
            CalculationPerformed?.Invoke(this, e);
        }
        
        public int Add(int a, int b)
        {
            int result = a + b;
            OnCalculationPerformed(new CalculationEventArgs("Addition", a, b, result));
            return result;
        }
        
        public int Subtract(int a, int b)
        {
            int result = a - b;
            OnCalculationPerformed(new CalculationEventArgs("Subtraction", a, b, result));
            return result;
        }
        
        public int Multiply(int a, int b)
        {
            int result = a * b;
            OnCalculationPerformed(new CalculationEventArgs("Multiplication", a, b, result));
            return result;
        }
        
        public int Divide(int a, int b)
        {
            if (b == 0) throw new DivideByZeroException();
            int result = a / b;
            OnCalculationPerformed(new CalculationEventArgs("Division", a, b, result));
            return result;
        }
        
        // Method that accepts a delegate as a parameter
        public int PerformOperation(int a, int b, BinaryOperation operation)
        {
            return operation(a, b);
        }
    }
    
    // 4. Another class for notification system
    public class NotificationService
    {
        // Using built-in Action delegate
        private Action<string> _notificationHandler;
        
        public void Subscribe(Action<string> handler)
        {
            // Multicast delegate - combining delegates
            _notificationHandler += handler;
        }
        
        public void Unsubscribe(Action<string> handler)
        {
            _notificationHandler -= handler;
        }
        
        public void SendNotification(string message)
        {
            Console.WriteLine($"Sending notification: {message}");
            _notificationHandler?.Invoke(message);
        }
    }
    
    // 5. Main demo class
    public class DelegateEventDemo
    {
        public void Demo()
        {
            Console.WriteLine("=== C# Delegates and Events Demo ===\n");
            
            // Section 1: Basic Delegates
            Console.WriteLine("1. Basic Delegates:");
            
            // Creating delegate instance with named method
            BinaryOperation addDelegate = AddNumbers;
            int sum = addDelegate(10, 5);
            Console.WriteLine($"  addDelegate(10, 5) = {sum}");
            
            BinaryOperation multiplyDelegate = MultiplyNumbers;
            int product = multiplyDelegate(10, 5);
            Console.WriteLine($"  multiplyDelegate(10, 5) = {product}");
            
            // Section 2: Anonymous Methods
            Console.WriteLine("\n2. Anonymous Methods:");
            
            BinaryOperation subtractDelegate = delegate(int a, int b)
            {
                return a - b;
            };
            Console.WriteLine($"  subtractDelegate(10, 5) = {subtractDelegate(10, 5)}");
            
            // Section 3: Lambda Expressions
            Console.WriteLine("\n3. Lambda Expressions:");
            
            BinaryOperation divideDelegate = (a, b) => a / b;
            Console.WriteLine($"  divideDelegate(10, 5) = {divideDelegate(10, 5)}");
            
            // Lambda with multiple statements
            BinaryOperation complexOperation = (a, b) =>
            {
                int temp = a * 2;
                return temp + b;
            };
            Console.WriteLine($"  complexOperation(10, 5) = {complexOperation(10, 5)}");
            
            // Section 4: Multicast Delegates
            Console.WriteLine("\n4. Multicast Delegates:");
            
            NotificationDelegate notificationChain = null;
            notificationChain += SendToConsole;
            notificationChain += SendToLog;
            notificationChain += SendToEmail;
            
            Console.WriteLine("  Invoking multicast delegate with 3 methods:");
            notificationChain("Important system update!");
            
            // Section 5: Built-in Delegate Types
            Console.WriteLine("\n5. Built-in Delegate Types:");
            
            // Action - for methods that don't return a value
            Action<string> printAction = (message) => Console.WriteLine($"  Action says: {message}");
            printAction("Hello from Action!");
            
            // Func - for methods that return a value
            Func<int, int, string> formatFunc = (x, y) => $"  Func result: {x} + {y} = {x + y}";
            Console.WriteLine(formatFunc(7, 3));
            
            // Predicate - for methods that return bool
            Predicate<int> isEvenPredicate = (num) => num % 2 == 0;
            Console.WriteLine($"  Is 10 even? {isEvenPredicate(10)}");
            Console.WriteLine($"  Is 7 even? {isEvenPredicate(7)}");
            
            // Section 6: Events
            Console.WriteLine("\n6. Events:");
            
            Calculator calculator = new Calculator();
            
            // Subscribing to the event
            calculator.CalculationPerformed += Calculator_CalculationPerformed;
            calculator.CalculationPerformed += (sender, e) =>
            {
                Console.WriteLine($"  Lambda handler: Operation logged to database");
            };
            
            // Using the calculator
            Console.WriteLine("  Performing calculations:");
            calculator.Add(15, 3);
            calculator.Multiply(4, 6);
            calculator.Subtract(20, 8);
            
            // Unsubscribing from event
            calculator.CalculationPerformed -= Calculator_CalculationPerformed;
            
            // Section 7: Delegate as Parameter
            Console.WriteLine("\n7. Delegates as Parameters:");
            
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            
            // Using delegate with LINQ-like filtering
            var evenNumbers = FilterNumbers(numbers, IsEven);
            Console.WriteLine($"  Even numbers: {string.Join(", ", evenNumbers)}");
            
            // Using lambda directly
            var oddNumbers = FilterNumbers(numbers, n => n % 2 == 1);
            Console.WriteLine($"  Odd numbers: {string.Join(", ", oddNumbers)}");
            
            // Section 8: Practical Example - Notification System
            Console.WriteLine("\n8. Practical Example - Notification System:");
            
            NotificationService notificationService = new NotificationService();
            
            // Subscribe multiple handlers
            notificationService.Subscribe(message => Console.WriteLine($"  Email sent: {message}"));
            notificationService.Subscribe(message => Console.WriteLine($"  SMS sent: {message}"));
            notificationService.Subscribe(message => Console.WriteLine($"  Push notification: {message}"));
            
            // Send notification
            notificationService.SendNotification("Server maintenance scheduled");
            
            // Section 9: Delegate Combinations
            Console.WriteLine("\n9. Delegate Combinations:");
            
            BinaryOperation op1 = AddNumbers;
            BinaryOperation op2 = MultiplyNumbers;
            
            // Combine delegates
            BinaryOperation combined = (BinaryOperation)Delegate.Combine(op1, op2);
            Console.WriteLine("  Note: Combined delegates can be invoked, but only last result is returned");
            
            // Get invocation list
            Console.WriteLine("  Delegate invocation list:");
            foreach (var del in combined.GetInvocationList())
            {
                Console.WriteLine($"    - {del.Method.Name}");
            }
            
            Console.WriteLine("\n=== Delegates and Events Demo Complete ===");
        }
        
        // Named methods for delegates
        private static int AddNumbers(int a, int b)
        {
            return a + b;
        }
        
        private int InstanceAdd(int a, int b)
        {
            return a + b + 1; // Slight difference to show it's different
        }
        
        private static int MultiplyNumbers(int a, int b)
        {
            return a * b;
        }
        
        private static bool IsEven(int number)
        {
            return number % 2 == 0;
        }
        
        // Notification methods
        private void SendToConsole(string message)
        {
            Console.WriteLine($"    Console: {message}");
        }
        
        private void SendToLog(string message)
        {
            Console.WriteLine($"    Log: {DateTime.Now:HH:mm:ss} - {message}");
        }
        
        private void SendToEmail(string message)
        {
            Console.WriteLine($"    Email: Sending '{message}' to admin@example.com");
        }
        
        // Event handler method
        private void Calculator_CalculationPerformed(object sender, CalculationEventArgs e)
        {
            Console.WriteLine($"  Event handler: {e.Operand1} {e.Operation.ToLower()} {e.Operand2} = {e.Result}");
        }
        
        // Method that accepts a delegate parameter
        private List<int> FilterNumbers(List<int> numbers, FilterDelegate filter)
        {
            List<int> result = new List<int>();
            foreach (int number in numbers)
            {
                if (filter(number))
                {
                    result.Add(number);
                }
            }
            return result;
        }
        
        // Additional method to demonstrate delegate compatibility
        public void DemonstrateDelegateCompatibility()
        {
            Console.WriteLine("\n--- Additional: Delegate Compatibility ---");
            
            // Instance methods vs static methods
            // Static method delegate assignment
            BinaryOperation staticDelegate = AddNumbers;
            Console.WriteLine($"  Static method delegate: {staticDelegate(5, 3)}");
            
            // Instance method delegate assignment
            BinaryOperation instanceDelegate = this.InstanceAdd;
            Console.WriteLine($"  Instance method delegate: {instanceDelegate(5, 3)}");
            
            // Method group conversion
            BinaryOperation methodGroup = AddNumbers;
            Console.WriteLine($"  Method group conversion: {methodGroup(8, 2)}");
            
            // Covariance and contravariance in delegates
            Func<object> getObject = GetString;
            Console.WriteLine($"  Covariance: {getObject()}");
            
            Action<string> action = AcceptObject;
            Console.WriteLine("  Contravariance demonstrated");
        }
        
        private static string GetString()
        {
            return "Hello from covariant delegate";
        }
        
        private static void AcceptObject(object obj)
        {
            Console.WriteLine($"  Accepting object: {obj}");
        }
    }
}