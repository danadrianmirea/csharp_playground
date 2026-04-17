using System;

class Program 
{
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

        BasicClass b = new BasicClass();
        int num = b.TestMethod();
        Console.WriteLine("num = " + num);

        DataTypesDemo demoTypes = new DataTypesDemo();
        demoTypes.Demo();

        ControlStructuresDemo demoControl = new ControlStructuresDemo();
        demoControl.Demo();
    }
}