using System;

namespace InheritanceDemo
{
    // Base class
    public class Vehicle
    {
        // Protected field - accessible to derived classes
        protected string make;
        protected int year;
        
        // Public property
        public string Make 
        { 
            get { return make; }
            set { make = value; }
        }
        
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        
        // Constructor
        public Vehicle(string make, int year)
        {
            this.make = make;
            this.year = year;
            Console.WriteLine($"Vehicle constructor called for {make} ({year})");
        }
        
        // Virtual method - can be overridden by derived classes
        public virtual void Start()
        {
            Console.WriteLine($"{make} vehicle starting...");
        }
        
        // Virtual method
        public virtual void Stop()
        {
            Console.WriteLine($"{make} vehicle stopping...");
        }
        
        // Regular method
        public void DisplayInfo()
        {
            Console.WriteLine($"Vehicle: {year} {make}");
        }
    }
    
    // Derived class 1 - Automobile (inherits from Vehicle)
    public class Automobile : Vehicle
    {
        // Additional field specific to Automobile
        public string model;
        public int speed;
        
        // Property
        public string Model
        {
            get { return model; }
            set { model = value; }
        }
        
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        
        // Constructor - calls base constructor using base()
        public Automobile(string make, string model, int year) : base(make, year)
        {
            this.model = model;
            this.speed = 0;
            Console.WriteLine($"Automobile constructor called for {make} {model}");
        }
        
        // Override virtual method
        public override void Start()
        {
            Console.WriteLine($"{make} {model} automobile starting with key ignition...");
            speed = 10; // Automobile starts moving slowly
        }
        
        // Override virtual method
        public override void Stop()
        {
            Console.WriteLine($"{make} {model} automobile applying brakes...");
            speed = 0;
        }
        
        // New method specific to Automobile
        public void Accelerate(int increment)
        {
            speed += increment;
            Console.WriteLine($"{make} {model} accelerating to {speed} km/h");
        }
        
        // New method specific to Automobile
        public void Honk()
        {
            Console.WriteLine($"{make} {model} honking: Beep beep!");
        }
        
        // Hiding base method with new keyword (not recommended usually, but shown for demo)
        public new void DisplayInfo()
        {
            Console.WriteLine($"Automobile: {year} {make} {model} (Current speed: {speed} km/h)");
        }
        
        // Using base keyword to call base class method
        public void ShowBaseInfo()
        {
            base.DisplayInfo();
        }
    }
    
    // Derived class 2 - Motorcycle (inherits from Vehicle)
    public class Motorcycle : Vehicle
    {
        // Additional field
        private bool hasSideCar;
        
        // Property
        public bool HasSideCar
        {
            get { return hasSideCar; }
            set { hasSideCar = value; }
        }
        
        // Constructor
        public Motorcycle(string make, int year, bool hasSideCar = false) : base(make, year)
        {
            this.hasSideCar = hasSideCar;
            Console.WriteLine($"Motorcycle constructor called for {make} (Sidecar: {hasSideCar})");
        }
        
        // Override virtual method
        public override void Start()
        {
            Console.WriteLine($"{make} motorcycle starting with kickstart...");
        }
        
        // Override virtual method
        public override void Stop()
        {
            Console.WriteLine($"{make} motorcycle stopping...");
        }
        
        // New method specific to Motorcycle
        public void Wheelie()
        {
            Console.WriteLine($"{make} motorcycle doing a wheelie!");
        }
        
        // Override DisplayInfo
        public override string ToString()
        {
            string sidecarInfo = hasSideCar ? "with sidecar" : "without sidecar";
            return $"Motorcycle: {year} {make} {sidecarInfo}";
        }
    }
    
    // Derived class 3 - ElectricVehicle (inherits from Automobile, which inherits from Vehicle)
    public class ElectricVehicle : Automobile
    {
        // Additional field
        private int batteryLevel;
        
        // Property
        public int BatteryLevel
        {
            get { return batteryLevel; }
            set 
            { 
                if (value >= 0 && value <= 100)
                    batteryLevel = value;
            }
        }
        
        // Constructor
        public ElectricVehicle(string make, string model, int year, int batteryLevel = 100) 
            : base(make, model, year)
        {
            this.batteryLevel = batteryLevel;
            Console.WriteLine($"ElectricVehicle constructor called for {make} {model} (Battery: {batteryLevel}%)");
        }
        
        // Override Start method
        public override void Start()
        {
            if (batteryLevel > 10)
            {
                Console.WriteLine($"{make} {model} electric vehicle starting silently...");
                Speed = 5; // Electric vehicles start slowly and quietly
                batteryLevel -= 5;
            }
            else
            {
                Console.WriteLine($"{make} {model} battery too low to start!");
            }
        }
        
        // Override Stop method with regenerative braking
        public override void Stop()
        {
            Console.WriteLine($"{make} {model} electric vehicle stopping with regenerative braking...");
            Speed = 0;
            // Regenerative braking adds some charge back
            if (batteryLevel < 95)
            {
                batteryLevel += 5;
                Console.WriteLine($"  Regenerative braking added 5% charge. Battery now at {batteryLevel}%");
            }
        }
        
        // New method specific to ElectricVehicle
        public void Charge(int amount)
        {
            batteryLevel += amount;
            if (batteryLevel > 100) batteryLevel = 100;
            Console.WriteLine($"{make} {model} charged to {batteryLevel}%");
        }
        
        // Override DisplayInfo
        public new void DisplayInfo()
        {
            Console.WriteLine($"Electric Vehicle: {Year} {Make} {Model} (Battery: {batteryLevel}%, Speed: {Speed} km/h)");
        }
    }
    
    // Demo class to demonstrate inheritance concepts
    public class InheritanceDemo
    {
        public void Demo()
        {
            Console.WriteLine("=== C# Inheritance Demo ===\n");
            
            // 1. Creating base class instance
            Console.WriteLine("1. Base Class (Vehicle):");
            Vehicle genericVehicle = new Vehicle("Generic", 2020);
            genericVehicle.Start();
            genericVehicle.Stop();
            genericVehicle.DisplayInfo();
            
            Console.WriteLine("\n2. Derived Class (Automobile):");
            // 2. Creating derived class instance
            Automobile myAuto = new Automobile("Toyota", "Camry", 2022);
            myAuto.Start();
            myAuto.Accelerate(50);
            myAuto.Honk();
            myAuto.DisplayInfo(); // Calls Automobile's DisplayInfo (hides base)
            myAuto.ShowBaseInfo(); // Calls base Vehicle's DisplayInfo
            myAuto.Stop();
            
            Console.WriteLine("\n3. Derived Class (Motorcycle):");
            // 3. Creating another derived class instance
            Motorcycle myBike = new Motorcycle("Harley-Davidson", 2021, false);
            myBike.Start();
            myBike.Wheelie();
            Console.WriteLine(myBike.ToString()); // Using overridden ToString
            myBike.Stop();
            
            Console.WriteLine("\n4. Multi-level Inheritance (ElectricVehicle inherits from Automobile):");
            // 4. Multi-level inheritance
            ElectricVehicle myTesla = new ElectricVehicle("Tesla", "Model 3", 2023, 80);
            myTesla.Start();
            myTesla.Accelerate(30);
            myTesla.DisplayInfo();
            myTesla.Stop();
            myTesla.Charge(30);
            myTesla.DisplayInfo();
            
            Console.WriteLine("\n5. Polymorphism - Using base class reference:");
            // 5. Polymorphism demonstration
            Console.WriteLine("Polymorphism examples:");
            
            // Array of base class references pointing to different derived objects
            Vehicle[] vehicles = new Vehicle[4];
            vehicles[0] = new Vehicle("Generic", 2020);
            vehicles[1] = new Automobile("Honda", "Civic", 2021);
            vehicles[2] = new Motorcycle("Yamaha", 2022, true);
            vehicles[3] = new ElectricVehicle("Nissan", "Leaf", 2023, 90);
            
            foreach (Vehicle vehicle in vehicles)
            {
                Console.WriteLine($"\nProcessing {vehicle.GetType().Name}:");
                vehicle.Start(); // Polymorphic call - calls appropriate override
                vehicle.Stop();  // Polymorphic call
                vehicle.DisplayInfo();
                
                // Type checking and casting
                if (vehicle is Automobile auto)
                {
                    auto.Honk(); // Can call Automobile-specific method
                }
                
                if (vehicle is Motorcycle bike)
                {
                    bike.Wheelie(); // Can call Motorcycle-specific method
                }
            }
            
            Console.WriteLine("\n6. Base class reference to derived object:");
            // 6. Base class reference to derived object
            Vehicle vehicleRef = new Automobile("Ford", "Mustang", 2020);
            vehicleRef.Start(); // Calls Automobile's Start (virtual dispatch)
            vehicleRef.Stop();  // Calls Automobile's Stop
            
            // Can't call Automobile-specific methods without casting
            // vehicleRef.Honk(); // Compile error
            
            // Safe casting with 'as' operator
            Automobile autoRef = vehicleRef as Automobile;
            if (autoRef != null)
            {
                autoRef.Honk();
            }
            
            Console.WriteLine("\n7. Abstract concept demonstration (not using abstract classes):");
            // 7. Showing how virtual/override enables polymorphic behavior
            Console.WriteLine("All vehicles share common interface but have different implementations.");
            
            Console.WriteLine("\n=== Inheritance Demo Complete ===");
        }
        
        // Additional method to demonstrate inheritance in method parameters
        public void ServiceVehicle(Vehicle vehicle)
        {
            Console.WriteLine($"\nServicing {vehicle.GetType().Name}:");
            Console.WriteLine($"  Make: {vehicle.Make}, Year: {vehicle.Year}");
            
            // Can call virtual methods
            vehicle.Start();
            vehicle.Stop();
        }
    }
}