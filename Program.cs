// See https://aka.ms/new-console-template for more information
using EfDemo;
using Microsoft.EntityFrameworkCore;

using var dbContext = new EfDemoDbContext();

Console.WriteLine("[Migrate] Start");
dbContext.Database.Migrate();
await dbContext.Database.ExecuteSqlRawAsync("delete [dbo].[Addresses]");
await dbContext.Database.ExecuteSqlRawAsync("delete [dbo].[Customers]");
Console.WriteLine("[Migrate] End");

Console.WriteLine("[Seed] Start");

var customer = new Customer
{
    Name = "John Doe",
    Addresses = new List<Address>
    {
        new Address { Street = "123 Main St" },
        new Address { Street = "456 Main St" }
    }
};
dbContext.Add(customer);
dbContext.SaveChanges();

customer.Name = "John Doe Jr.";
dbContext.Update(customer);
dbContext.SaveChanges();

dbContext.Entry(customer).Property("DeletedOn").CurrentValue = DateTime.UtcNow;
dbContext.SaveChanges();

var customer2 = new Customer
{
    Name = "Jane Doe",
    Addresses = new List<Address>
    {
        new Address { Street = "123 Main St" },
        new Address { Street = "456 Main St" }
    }
};
dbContext.Add(customer2);
dbContext.SaveChanges();

Console.WriteLine("[Seed] End");

Console.WriteLine("[Queries] Start");

var customersWithAddresses = await dbContext.Set<Customer>()
    .ToListAsync();

var customersWithoutAddresses = await dbContext.Set<Customer>()
    .IgnoreAutoIncludes()
    .ToListAsync();

var customersAlsoDeleted = await dbContext.Set<Customer>()
    .IgnoreQueryFilters()
    .ToListAsync();

var customersHistory = await dbContext.Set<Customer>()
    .TemporalAll()
    .IgnoreAutoIncludes()
    .ToListAsync();

var allCustomersHistory = await dbContext.Set<Customer>()
    .TemporalAll()
    .IgnoreAutoIncludes()
    .IgnoreQueryFilters()
    .ToListAsync();

var customersHistoryWithTimeframe = await dbContext.Set<Customer>()
    .TemporalFromTo(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31))
    .IgnoreAutoIncludes()
    .ToListAsync();

var customersHistoryWithShadowProperties = await dbContext.Set<Customer>()
    .TemporalAll()
    .Where(c => EF.Property<DateTime>(c, "PeriodStart") >= DateTime.Today)
    .OrderBy(e => EF.Property<DateTime>(e, "PeriodEnd"))
    .IgnoreAutoIncludes()
    .ToListAsync();

var customersViaSoundEx = await dbContext.Set<Customer>()
    .Where(c => EfDemoDbContext.SoundEx(c.Name) == EfDemoDbContext.SoundEx("Jhon Do"))
    .ToListAsync();
Console.WriteLine("[Queries] End");

Console.WriteLine("Press a key to exit...");
Console.ReadLine();
