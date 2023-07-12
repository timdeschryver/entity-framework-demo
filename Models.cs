namespace EfDemo;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Address> Addresses { get; set; }
}

public class Address
{
    public Guid Id { get; set; }
    public string Street { get; set; }
}