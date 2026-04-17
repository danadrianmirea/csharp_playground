using System;

class MathOperations
{
    // Method to add two numbers
    public int Add(int a, int b)
    {
        return a + b;
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
}