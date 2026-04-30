using Raylib_cs;
using System.Numerics;
using System.Reflection;

// Check what DrawTriangle methods exist
var methods = typeof(Raylib).GetMethods(BindingFlags.Public | BindingFlags.Static)
    .Where(m => m.Name.Contains("Triangle"))
    .Select(m => $"{m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");

Console.WriteLine("Triangle methods found:");
foreach (var m in methods)
{
    Console.WriteLine($"  {m}");
}
