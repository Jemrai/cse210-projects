using System;
using System.Collections.Generic;

public class Address
{
    private string street;
    private string city;
    private string state;
    private string country;

    public Address(string street, string city, string state, string country)
    {
        this.street = street;
        this.city = city;
        this.state = state;
        this.country = country;
    }

    public bool IsInUSA()
    {
        return country == "USA" || country == "United States";
    }

    public string GetFullAddress()
    {
        return $"{street}\n{city}, {state}\n{country}";
    }
}

public class Customer
{
    private string name;
    private Address address;

    public Customer(string name, Address address)
    {
        this.name = name;
        this.address = address;
    }

    public bool LivesInUSA()
    {
        return address.IsInUSA();
    }

    public string Name
    {
        get { return name; }
    }

    public Address Address
    {
        get { return address; }
    }
}

public class Product
{
    private string name;
    private string productId;
    private double price;
    private int quantity;

    public Product(string name, string productId, double price, int quantity)
    {
        this.name = name;
        this.productId = productId;
        this.price = price;
        this.quantity = quantity;
    }

    public double GetTotalCost()
    {
        return price * quantity;
    }

    public string Name
    {
        get { return name; }
    }

    public string ProductId
    {
        get { return productId; }
    }
}

public class Order
{
    private List<Product> products;
    private Customer customer;

    public Order(Customer customer)
    {
        this.customer = customer;
        this.products = new List<Product>();
    }

    public void AddProduct(Product product)
    {
        products.Add(product);
    }

    public double CalculateTotalPrice()
    {
        double subtotal = 0.0;
        foreach (Product product in products)
        {
            subtotal += product.GetTotalCost();
        }
        double shippingCost = customer.LivesInUSA() ? 5.0 : 35.0;
        return subtotal + shippingCost;
    }

    public string GetPackingLabel()
    {
        string label = "";
        foreach (Product product in products)
        {
            label += $"{product.Name} ({product.ProductId})\n";
        }
        return label.TrimEnd('\n');
    }

    public string GetShippingLabel()
    {
        return $"{customer.Name}\n{customer.Address.GetFullAddress()}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Address usAddress = new Address("123 Main St", "Seattle", "WA", "USA");
        Customer usCustomer = new Customer("John Doe", usAddress);
        Order usOrder = new Order(usCustomer);
        usOrder.AddProduct(new Product("Laptop", "LAP001", 999.99, 1));
        usOrder.AddProduct(new Product("Mouse", "MOU001", 25.00, 2));
        usOrder.AddProduct(new Product("Keyboard", "KEY001", 75.00, 1));

        Address intAddress = new Address("456 Elm St", "Toronto", "ON", "Canada");
        Customer intCustomer = new Customer("Jane Smith", intAddress);
        Order intOrder = new Order(intCustomer);
        intOrder.AddProduct(new Product("Tablet", "TAB001", 499.99, 1));
        intOrder.AddProduct(new Product("Headphones", "HEA001", 150.00, 1));
        intOrder.AddProduct(new Product("Case", "CAS001", 30.00, 2));

        Console.WriteLine("US Order:");
        Console.WriteLine("Packing Label:");
        Console.WriteLine(usOrder.GetPackingLabel());
        Console.WriteLine("\nShipping Label:");
        Console.WriteLine(usOrder.GetShippingLabel());
        Console.WriteLine($"\nTotal Price: {usOrder.CalculateTotalPrice():C}");
        Console.WriteLine();

        Console.WriteLine("International Order:");
        Console.WriteLine("Packing Label:");
        Console.WriteLine(intOrder.GetPackingLabel());
        Console.WriteLine("\nShipping Label:");
        Console.WriteLine(intOrder.GetShippingLabel());
        Console.WriteLine($"\nTotal Price: {intOrder.CalculateTotalPrice():C}");
    }
}