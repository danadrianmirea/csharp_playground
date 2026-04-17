using System;

class ControlStructuresDemo
{
    // Non-static method to demonstrate control structures
    public void Demo()
    {
        // If-Else Statement
        int number = 10;
        if (number > 0)
        {
            Console.WriteLine("Number is positive.");
        }
        else
        {
            Console.WriteLine("Number is non-positive.");
        }

        // Switch Statement
        string day = "Wednesday";
        switch (day)
        {
            case "Monday":
                Console.WriteLine("Today is Monday.");
                break;
            case "Tuesday":
                Console.WriteLine("Today is Tuesday.");
                break;
            case "Wednesday":
                Console.WriteLine("Today is Wednesday.");
                break;
            default:
                Console.WriteLine("Unknown day.");
                break;
        }

        // For Loop
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine("For loop iteration: " + i);
        }

        // While Loop
        int count = 1;
        while (count <= 3)
        {
            Console.WriteLine("While loop iteration: " + count);
            count++;
        }

        // Do-While Loop
        int countD = 1;
        do
        {
            Console.WriteLine("Do-While loop iteration: " + countD);
            countD++;
        } while (countD <= 3);
    }
}
