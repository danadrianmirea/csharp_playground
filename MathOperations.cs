using System;

class MathOperations
{
    // Method to add two numbers
    public int Add(int a, int b)
    {
        return a + b;
    }
    public int Add(int a, int b, int c)
    {
        return a + b + c;
    }

    // Method to subtract two numbers
    public int Subtract(int a, int b)
    {
        return a - b;
    }

    // Method to multiply two numbers
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    // Method to divide two numbers
    public double Divide(double a, double b)
    {
        if (b != 0)
        {
            return a / b;
        }
        else
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }
    }

    // Method to calculate the remainder of a division
    public int Modulus(int a, int b)
    {
        return a % b;
    }

    // Method to add three integers

    // Method to multiply two integers
    // Method to swap the values of two integers using ref parameters
    public void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }

    // Method to find the maximum of two integers
    public int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    // Method to find the maximum of three integers
    public int Max(int a, int b, int c)
    {
        return Max(Max(a, b), c);
    }

    // Method to find the minimum of two integers
    public int Min(int a, int b)
    {
        return a < b ? a : b;
    }

    // Method to find the minimum of three integers
    public int Min(int a, int b, int c)
    {
        return Min(Min(a, b), c);
    }

    // Method to calculate the power of a number
    public double Power(double baseNum, int exponent)
    {
        double result = 1;
        for (int i = 0; i < exponent; i++)
        {
            result *= baseNum;
        }
        return result;
    }

    // Method to calculate the factorial of a number using out parameters
    public void Factorial(int n, out int result)
    {
        if (n < 0)
        {
            throw new ArgumentException("Number must be non-negative");
        }
        result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
    }    

    public void Demo()
    {
        MathOperations mathOps = new MathOperations();

        // Using Add methods
        Console.WriteLine($"Add(2, 3): {mathOps.Add(2, 3)}");
        Console.WriteLine($"Add(2, 3, 4): {mathOps.Add(2, 3, 4)}");

        // Using Multiply method
        Console.WriteLine($"Multiply(2, 3): {mathOps.Multiply(2, 3)}");

        // Using Swap method
        int a = 5, b = 10;
        Console.WriteLine($"Before Swap: a = {a}, b = {b}");
        mathOps.Swap(ref a, ref b);
        Console.WriteLine($"After Swap: a = {a}, b = {b}");

        // Using Max methods
        Console.WriteLine($"Max(2, 3): {mathOps.Max(2, 3)}");
        Console.WriteLine($"Max(2, 3, 4): {mathOps.Max(2, 3, 4)}");

        // Using Min methods
        Console.WriteLine($"Min(2, 3): {mathOps.Min(2, 3)}");
        Console.WriteLine($"Min(2, 3, 4): {mathOps.Min(2, 3, 4)}");

        // Using Power method
        Console.WriteLine($"Power(2, 3): {mathOps.Power(2, 3)}");

        // Using Factorial method
        int factorialResult;
        mathOps.Factorial(5, out factorialResult);
        Console.WriteLine($"Factorial(5): {factorialResult}");        
    }
}