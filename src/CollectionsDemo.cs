using System;
using System.Collections.Generic;

class CollectionsDemo
{
    public void Demo()
    {
        Console.WriteLine("=== C# Collections Demo ===");
        
        // 1. Arrays
        Console.WriteLine("\n1. Arrays:");
        int[] numbersArray = new int[] { 1, 2, 3, 4, 5 };
        Console.WriteLine("Integer array:");
        for (int i = 0; i < numbersArray.Length; i++)
        {
            Console.WriteLine($"  numbersArray[{i}] = {numbersArray[i]}");
        }
        
        string[] namesArray = { "Alice", "Bob", "Charlie" };
        Console.WriteLine("\nString array:");
        foreach (string name in namesArray)
        {
            Console.WriteLine($"  Name: {name}");
        }
        
        // 2. Lists (dynamic arrays)
        Console.WriteLine("\n2. Lists (List<T>):");
        List<string> fruits = new List<string>();
        fruits.Add("Apple");
        fruits.Add("Banana");
        fruits.Add("Cherry");
        fruits.Add("Date");
        
        Console.WriteLine($"Fruits list has {fruits.Count} items:");
        foreach (string fruit in fruits)
        {
            Console.WriteLine($"  - {fruit}");
        }
        
        // List operations
        fruits.Remove("Banana");
        Console.WriteLine($"\nAfter removing 'Banana', list has {fruits.Count} items:");
        foreach (string fruit in fruits)
        {
            Console.WriteLine($"  - {fruit}");
        }
        
        bool containsApple = fruits.Contains("Apple");
        Console.WriteLine($"\nList contains 'Apple': {containsApple}");
        
        // 3. Dictionaries (key-value pairs)
        Console.WriteLine("\n3. Dictionaries (Dictionary<TKey, TValue>):");
        Dictionary<string, int> ages = new Dictionary<string, int>();
        ages["John"] = 25;
        ages["Sarah"] = 30;
        ages["Mike"] = 28;
        ages.Add("Emma", 22); // Alternative way to add
        
        Console.WriteLine("Age dictionary:");
        foreach (KeyValuePair<string, int> entry in ages)
        {
            Console.WriteLine($"  {entry.Key} is {entry.Value} years old");
        }
        
        // Dictionary operations
        bool hasJohn = ages.ContainsKey("John");
        Console.WriteLine($"\nDictionary contains key 'John': {hasJohn}");
        
        if (ages.TryGetValue("Sarah", out int sarahAge))
        {
            Console.WriteLine($"Sarah's age (retrieved via TryGetValue): {sarahAge}");
        }
        
        // 4. Stack (LIFO - Last In First Out)
        Console.WriteLine("\n4. Stack (LIFO):");
        Stack<string> stack = new Stack<string>();
        stack.Push("First");
        stack.Push("Second");
        stack.Push("Third");
        
        Console.WriteLine("Stack contents (from top to bottom):");
        foreach (string item in stack)
        {
            Console.WriteLine($"  {item}");
        }
        
        string popped = stack.Pop();
        Console.WriteLine($"\nPopped item: {popped}");
        Console.WriteLine($"Stack now has {stack.Count} items");
        
        // 5. Queue (FIFO - First In First Out)
        Console.WriteLine("\n5. Queue (FIFO):");
        Queue<string> queue = new Queue<string>();
        queue.Enqueue("First");
        queue.Enqueue("Second");
        queue.Enqueue("Third");
        
        Console.WriteLine("Queue contents:");
        foreach (string item in queue)
        {
            Console.WriteLine($"  {item}");
        }
        
        string dequeued = queue.Dequeue();
        Console.WriteLine($"\nDequeued item: {dequeued}");
        Console.WriteLine($"Queue now has {queue.Count} items");
        
        // 6. HashSet (unique elements)
        Console.WriteLine("\n6. HashSet (unique elements):");
        HashSet<string> uniqueNames = new HashSet<string>();
        uniqueNames.Add("Alice");
        uniqueNames.Add("Bob");
        uniqueNames.Add("Alice"); // Duplicate - won't be added
        
        Console.WriteLine($"HashSet has {uniqueNames.Count} unique names:");
        foreach (string name in uniqueNames)
        {
            Console.WriteLine($"  {name}");
        }
        
        // 7. List methods demonstration
        Console.WriteLine("\n7. List Methods Demonstration:");
        List<int> scores = new List<int> { 85, 92, 78, 95, 88 };
        
        Console.WriteLine("Original scores:");
        foreach (int score in scores)
        {
            Console.Write($"{score} ");
        }
        
        scores.Sort();
        Console.WriteLine("\nSorted scores:");
        foreach (int score in scores)
        {
            Console.Write($"{score} ");
        }
        
        int maxScore = scores[scores.Count - 1];
        int minScore = scores[0];
        Console.WriteLine($"\nHighest score: {maxScore}, Lowest score: {minScore}");
        
        // 8. Initialization syntax variations
        Console.WriteLine("\n8. Collection Initialization Syntax:");
        
        // List with initializer
        List<string> colors = new List<string> { "Red", "Green", "Blue" };
        Console.WriteLine($"Colors list: {string.Join(", ", colors)}");
        
        // Dictionary with initializer
        Dictionary<int, string> numberNames = new Dictionary<int, string>
        {
            { 1, "One" },
            { 2, "Two" },
            { 3, "Three" }
        };
        Console.WriteLine($"Number names: 1 = {numberNames[1]}, 2 = {numberNames[2]}");
        
        Console.WriteLine("\n=== Collections Demo Complete ===");
    }
    
    // Additional method to demonstrate collection as parameter and return type
    public List<int> GetEvenNumbers(int count)
    {
        List<int> evenNumbers = new List<int>();
        for (int i = 1; i <= count; i++)
        {
            evenNumbers.Add(i * 2);
        }
        return evenNumbers;
    }
    
    public void ProcessCollection(List<string> items)
    {
        Console.WriteLine($"\nProcessing {items.Count} items:");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"  Item {i + 1}: {items[i]}");
        }
    }
}