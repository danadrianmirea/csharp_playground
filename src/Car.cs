public class Car
{
    // Define private fields
    private string make;
    private string model;
    private int year;
    private int speed;

    // Constructor to initialize the Car object
    public Car(string make, string model, int year)
    {
        this.make = make;
        this.model = model;
        this.year = year;
        this.speed = 0;
    }

    // Public property to get and set the Make of the car
    public string Make
    {
        get { return make; }
        set { make = value; }
    }

    // Public property to get and set the Model of the car
    public string Model
    {
        get { return model; }
        set { model = value; }
    }

    // Public property to get and set the Year of the car
    public int Year
    {
        get { return year; }
        set { year = value; }
    }

    // Public property to get and set the Speed of the car
    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // Method to accelerate the car
    public void Accelerate(int increment)
    {
        speed += increment;
        Console.WriteLine($"{make} {model} is now going {speed} mph.");
    }

    // Method to brake the car
    public void Brake(int decrement)
    {
        speed -= decrement;
        if (speed < 0) speed = 0;
        Console.WriteLine($"{make} {model} is now going {speed} mph.");
    }

    // Method to display car information
    public void DisplayInfo()
    {
        Console.WriteLine($"Car: {year} {make} {model}");
    }
}