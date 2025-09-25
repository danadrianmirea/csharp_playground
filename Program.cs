using System;

class Program {

    static void SplitString()
    {
        var text = "Hello, World!";
        var result = text.Split(" ");
        foreach (var res in result)
        {
            Console.WriteLine(res);
        }
    }

    static void Main()
    {
        SplitString();
    }
}