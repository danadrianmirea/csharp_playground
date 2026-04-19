using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqDemo
{
    // Sample data classes for LINQ demonstrations
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
        public override string ToString()
        {
            return $"{Name} ({Category}) - ${Price:F2} [Stock: {Stock}]";
        }
    }
    
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Grade { get; set; } // A, B, C, D, F
        public List<string> Courses { get; set; }
        
        public override string ToString()
        {
            return $"{Name}, Age: {Age}, Grade: {Grade}";
        }
    }
    
    public class Order
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        
        public override string ToString()
        {
            return $"Order #{Id}: {Customer} - ${Amount:F2} on {Date:yyyy-MM-dd}";
        }
    }
    
    public class LinqDemo
    {
        public void Demo()
        {
            Console.WriteLine("=== C# LINQ (Language Integrated Query) Demo ===\n");
            
            // Initialize sample data
            var products = GetSampleProducts();
            var students = GetSampleStudents();
            var orders = GetSampleOrders();
            
            // Part 1: Basic LINQ Operations
            Console.WriteLine("1. BASIC LINQ OPERATIONS\n");
            
            // 1.1 Filtering with Where (Method Syntax)
            Console.WriteLine("1.1 Filtering - Products with price > 50 (Method Syntax):");
            var expensiveProducts = products.Where(p => p.Price > 50);
            foreach (var product in expensiveProducts)
            {
                Console.WriteLine($"  {product}");
            }
            
            // 1.2 Filtering with Where (Query Syntax)
            Console.WriteLine("\n1.2 Filtering - Products with price > 50 (Query Syntax):");
            var expensiveProductsQuery = from p in products
                                        where p.Price > 50
                                        select p;
            foreach (var product in expensiveProductsQuery)
            {
                Console.WriteLine($"  {product}");
            }
            
            // 1.3 Projection with Select
            Console.WriteLine("\n1.3 Projection - Product names only:");
            var productNames = products.Select(p => p.Name);
            Console.WriteLine($"  Product names: {string.Join(", ", productNames)}");
            
            // 1.4 Ordering
            Console.WriteLine("\n1.4 Ordering - Products by price (descending):");
            var orderedProducts = products.OrderByDescending(p => p.Price);
            foreach (var product in orderedProducts.Take(3))
            {
                Console.WriteLine($"  {product}");
            }
            
            // Part 2: Complex Object Queries
            Console.WriteLine("\n\n2. COMPLEX OBJECT QUERIES\n");
            
            // 2.1 Grouping
            Console.WriteLine("2.1 Grouping - Products by category:");
            var productsByCategory = products.GroupBy(p => p.Category);
            foreach (var group in productsByCategory)
            {
                Console.WriteLine($"  Category: {group.Key} ({group.Count()} products)");
                foreach (var product in group.Take(2))
                {
                    Console.WriteLine($"    - {product.Name}: ${product.Price:F2}");
                }
            }
            
            // 2.2 Aggregation
            Console.WriteLine("\n2.2 Aggregation - Product statistics:");
            var totalProducts = products.Count();
            var totalValue = products.Sum(p => p.Price * p.Stock);
            var avgPrice = products.Average(p => p.Price);
            var maxPrice = products.Max(p => p.Price);
            var minPrice = products.Min(p => p.Price);
            
            Console.WriteLine($"  Total products: {totalProducts}");
            Console.WriteLine($"  Total inventory value: ${totalValue:F2}");
            Console.WriteLine($"  Average price: ${avgPrice:F2}");
            Console.WriteLine($"  Max price: ${maxPrice:F2}");
            Console.WriteLine($"  Min price: ${minPrice:F2}");
            
            // 2.3 Complex filtering and projection
            Console.WriteLine("\n2.3 Complex query - Electronics in stock:");
            var electronicsInStock = from p in products
                                    where p.Category == "Electronics" && p.Stock > 0
                                    orderby p.Price descending
                                    select new { p.Name, p.Price, p.Stock };
            
            Console.WriteLine("  Electronics in stock:");
            foreach (var item in electronicsInStock)
            {
                Console.WriteLine($"    {item.Name}: ${item.Price:F2} (Stock: {item.Stock})");
            }
            
            // Part 3: Student Data Queries
            Console.WriteLine("\n\n3. STUDENT DATA QUERIES\n");
            
            // 3.1 Multiple conditions
            Console.WriteLine("3.1 Students with grade A or B, age > 20:");
            var topStudents = students.Where(s => (s.Grade == "A" || s.Grade == "B") && s.Age > 20);
            foreach (var student in topStudents)
            {
                Console.WriteLine($"  {student}");
            }
            
            // 3.2 Working with collections within objects
            Console.WriteLine("\n3.2 Students taking 'Mathematics':");
            var mathStudents = students.Where(s => s.Courses.Contains("Mathematics"));
            foreach (var student in mathStudents)
            {
                Console.WriteLine($"  {student.Name} - Courses: {string.Join(", ", student.Courses)}");
            }
            
            // 3.3 Grouping with aggregation
            Console.WriteLine("\n3.3 Average age by grade:");
            var avgAgeByGrade = students.GroupBy(s => s.Grade)
                                       .Select(g => new 
                                       { 
                                           Grade = g.Key, 
                                           AvgAge = g.Average(s => s.Age),
                                           Count = g.Count()
                                       })
                                       .OrderBy(g => g.Grade);
            
            foreach (var group in avgAgeByGrade)
            {
                Console.WriteLine($"  Grade {group.Grade}: Average age = {group.AvgAge:F1} ({group.Count} students)");
            }
            
            // Part 4: Advanced LINQ Features
            Console.WriteLine("\n\n4. ADVANCED LINQ FEATURES\n");
            
            // 4.1 First vs FirstOrDefault
            Console.WriteLine("4.1 First vs FirstOrDefault:");
            try
            {
                var firstExpensive = products.First(p => p.Price > 1000);
                Console.WriteLine($"  First product > $1000: {firstExpensive.Name}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"  First: No product found with price > $1000 (throws exception)");
            }
            
            var firstOrDefaultExpensive = products.FirstOrDefault(p => p.Price > 1000);
            Console.WriteLine($"  FirstOrDefault: {(firstOrDefaultExpensive != null ? firstOrDefaultExpensive.Name : "No product found (returns null)")}");
            
            // 4.2 Any, All, Contains
            Console.WriteLine("\n4.2 Any, All, Contains:");
            bool hasExpensiveProducts = products.Any(p => p.Price > 500);
            bool allHaveStock = products.All(p => p.Stock > 0);
            bool containsLaptop = products.Any(p => p.Name.Contains("Laptop"));
            
            Console.WriteLine($"  Any product > $500: {hasExpensiveProducts}");
            Console.WriteLine($"  All products have stock: {allHaveStock}");
            Console.WriteLine($"  Contains 'Laptop': {containsLaptop}");
            
            // 4.3 Skip and Take (Pagination)
            Console.WriteLine("\n4.3 Pagination with Skip and Take:");
            Console.WriteLine("  Page 1 (first 3 products):");
            var page1 = products.OrderBy(p => p.Id).Take(3);
            foreach (var product in page1)
            {
                Console.WriteLine($"    {product.Name}");
            }
            
            Console.WriteLine("  Page 2 (next 3 products):");
            var page2 = products.OrderBy(p => p.Id).Skip(3).Take(3);
            foreach (var product in page2)
            {
                Console.WriteLine($"    {product.Name}");
            }
            
            // 4.4 Distinct, Union, Intersect, Except
            Console.WriteLine("\n4.4 Set Operations:");
            var categories1 = new List<string> { "Electronics", "Books", "Clothing" };
            var categories2 = new List<string> { "Books", "Clothing", "Food" };
            
            Console.WriteLine($"  Categories1: {string.Join(", ", categories1)}");
            Console.WriteLine($"  Categories2: {string.Join(", ", categories2)}");
            Console.WriteLine($"  Union (all unique): {string.Join(", ", categories1.Union(categories2))}");
            Console.WriteLine($"  Intersect (common): {string.Join(", ", categories1.Intersect(categories2))}");
            Console.WriteLine($"  Except (in 1 but not 2): {string.Join(", ", categories1.Except(categories2))}");
            
            // Part 5: Order Data Queries
            Console.WriteLine("\n\n5. ORDER DATA QUERIES\n");
            
            // 5.1 Date filtering
            Console.WriteLine("5.1 Recent orders (last 30 days):");
            var recentDate = DateTime.Now.AddDays(-30);
            var recentOrders = orders.Where(o => o.Date > recentDate);
            
            foreach (var order in recentOrders)
            {
                Console.WriteLine($"  {order}");
            }
            
            // 5.2 Customer order summary
            Console.WriteLine("\n5.2 Customer order summary:");
            var customerSummary = orders.GroupBy(o => o.Customer)
                                       .Select(g => new
                                       {
                                           Customer = g.Key,
                                           TotalOrders = g.Count(),
                                           TotalAmount = g.Sum(o => o.Amount),
                                           AvgOrder = g.Average(o => o.Amount)
                                       })
                                       .OrderByDescending(c => c.TotalAmount);
            
            foreach (var summary in customerSummary)
            {
                Console.WriteLine($"  {summary.Customer}: {summary.TotalOrders} orders, Total: ${summary.TotalAmount:F2}, Avg: ${summary.AvgOrder:F2}");
            }
            
            // Part 6: Performance and Best Practices
            Console.WriteLine("\n\n6. PERFORMANCE AND BEST PRACTICES\n");
            
            // 6.1 Deferred execution demonstration
            Console.WriteLine("6.1 Deferred Execution:");
            var deferredQuery = products.Where(p => p.Price > 30).Select(p => p.Name);
            Console.WriteLine("  Query defined but not executed yet...");
            
            // Modify the source collection
            products.Add(new Product { Id = 100, Name = "New Product", Category = "Electronics", Price = 99.99m, Stock = 10 });
            
            Console.WriteLine("  After adding a new product, executing query:");
            foreach (var name in deferredQuery)
            {
                Console.WriteLine($"    {name}");
            }
            
            // 6.2 Immediate execution with ToList()
            Console.WriteLine("\n6.2 Immediate Execution (ToList()):");
            var immediateList = products.Where(p => p.Price > 30).Select(p => p.Name).ToList();
            Console.WriteLine($"  Query executed immediately, result count: {immediateList.Count}");
            
            // Part 7: LINQ Method Chain Examples
            Console.WriteLine("\n\n7. LINQ METHOD CHAIN EXAMPLES\n");
            
            // 7.1 Complex chain
            Console.WriteLine("7.1 Complex method chain - Top 3 expensive products in stock:");
            var topExpensiveInStock = products
                .Where(p => p.Stock > 0)
                .OrderByDescending(p => p.Price)
                .Take(3)
                .Select(p => new { p.Name, p.Price, p.Category });
            
            foreach (var item in topExpensiveInStock)
            {
                Console.WriteLine($"  {item.Name} ({item.Category}): ${item.Price:F2}");
            }
            
            Console.WriteLine("\n=== LINQ Demo Complete ===");
        }
        
        // Helper methods to create sample data
        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Stock = 5 },
                new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 29.99m, Stock = 20 },
                new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 79.99m, Stock = 15 },
                new Product { Id = 4, Name = "Monitor", Category = "Electronics", Price = 299.99m, Stock = 8 },
                new Product { Id = 5, Name = "C# Programming Book", Category = "Books", Price = 49.99m, Stock = 12 },
                new Product { Id = 6, Name = "T-Shirt", Category = "Clothing", Price = 19.99m, Stock = 30 },
                new Product { Id = 7, Name = "Jeans", Category = "Clothing", Price = 59.99m, Stock = 18 },
                new Product { Id = 8, Name = "Headphones", Category = "Electronics", Price = 149.99m, Stock = 10 },
                new Product { Id = 9, Name = "Webcam", Category = "Electronics", Price = 89.99m, Stock = 7 },
                new Product { Id = 10, Name = "Notebook", Category = "Stationery", Price = 9.99m, Stock = 50 }
            };
        }
        
        private List<Student> GetSampleStudents()
        {
            return new List<Student>
            {
                new Student { Id = 1, Name = "Alice Johnson", Age = 21, Grade = "A", Courses = new List<string> { "Mathematics", "Physics", "Computer Science" } },
                new Student { Id = 2, Name = "Bob Smith", Age = 22, Grade = "B", Courses = new List<string> { "Mathematics", "Chemistry", "Biology" } },
                new Student { Id = 3, Name = "Charlie Brown", Age = 20, Grade = "A", Courses = new List<string> { "Computer Science", "Physics", "Engineering" } },
                new Student { Id = 4, Name = "Diana Prince", Age = 23, Grade = "C", Courses = new List<string> { "History", "Literature", "Philosophy" } },
                new Student { Id = 5, Name = "Ethan Hunt", Age = 21, Grade = "B", Courses = new List<string> { "Mathematics", "Computer Science", "Statistics" } },
                new Student { Id = 6, Name = "Fiona Gallagher", Age = 19, Grade = "A", Courses = new List<string> { "Biology", "Chemistry", "Physics" } }
            };
        }
        
        private List<Order> GetSampleOrders()
        {
            return new List<Order>
            {
                new Order { Id = 1, Customer = "Alice Johnson", Amount = 299.99m, Date = DateTime.Now.AddDays(-10) },
                new Order { Id = 2, Customer = "Bob Smith", Amount = 89.99m, Date = DateTime.Now.AddDays(-25) },
                new Order { Id = 3, Customer = "Charlie Brown", Amount = 149.99m, Date = DateTime.Now.AddDays(-5) },
                new Order { Id = 4, Customer = "Alice Johnson", Amount = 59.99m, Date = DateTime.Now.AddDays(-15) },
                new Order { Id = 5, Customer = "Diana Prince", Amount = 199.99m, Date = DateTime.Now.AddDays(-40) },
                new Order { Id = 6, Customer = "Ethan Hunt", Amount = 79.99m, Date = DateTime.Now.AddDays(-2) },
                new Order { Id = 7, Customer = "Bob Smith", Amount = 129.99m, Date = DateTime.Now.AddDays(-20) }
            };
        }
    }
}