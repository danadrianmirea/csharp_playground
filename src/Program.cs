using System;
using System.Linq.Expressions;
using Graphical_OpenGL;
using RestAPI;
using SimpleCalculator;

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
        // SplitString();

        // BasicClass b = new BasicClass();
        // int num = b.TestMethod();
        // Console.WriteLine("num = " + num);

        // DataTypesDemo demoTypes = new DataTypesDemo();
        // demoTypes.Demo();

        // ControlStructuresDemo demoControl = new ControlStructuresDemo();
        // demoControl.Demo();

        // MathOperations math = new MathOperations();
        // int result = math.Add(2, 3);
        // Console.WriteLine("add result = " + result);
        // math.Demo();

        // Car car = new Car("BMW", "E90", 2009);
        // car.Accelerate(100);
        // car.DisplayInfo();

        // CollectionsDemo collectionsDemo = new CollectionsDemo();
        // collectionsDemo.Demo();

        // InheritanceDemo.InheritanceDemo inheritanceDemo = new InheritanceDemo.InheritanceDemo();
        // inheritanceDemo.Demo();

        // ExceptionHandlingDemo.ExceptionHandlingDemo exceptionDemo = new ExceptionHandlingDemo.ExceptionHandlingDemo();
        // exceptionDemo.Demo();

        // FileOperationsDemo.FileOperations fileOps = new FileOperationsDemo.FileOperations();
        // fileOps.Demo();

        // StringOperationsDemo.StringOperations stringOps = new StringOperationsDemo.StringOperations();
        // stringOps.Demo();

        // LinqDemo.LinqDemo linqDemo = new LinqDemo.LinqDemo();
        // linqDemo.Demo();
        
        // DelegateEventDemo.DelegateEventDemo delegateDemo = new DelegateEventDemo.DelegateEventDemo();
        // delegateDemo.Demo();

        //EquipmentRestApi server = new EquipmentRestApi(8080);
        //server.Run();

        //CubeDemo demo = new CubeDemo();
        //demo.Run();

        CalculatorUI calc = new CalculatorUI();
        calc.Run();
    }
}
