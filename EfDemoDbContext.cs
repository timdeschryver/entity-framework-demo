using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfDemo;

public class EfDemoDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = @"Server=.\;Database=EFDemo;Encrypt=False;MultipleActiveResultSets=true;Integrated Security=True;";
        optionsBuilder.UseSqlServer(connectionString);
        
        // optionsBuilder.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedOn").CurrentValue = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedOn").CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChanges();
    }

    [DbFunction(Name = "SoundEx", IsBuiltIn = true, IsNullable = false)]
    public static string SoundEx(string input)
    {
        throw new NotImplementedException();
    }
}

internal class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", o => o.IsTemporal());

        builder.Property<DateTime?>("DeletedOn");
        builder.Property<DateTime>("CreatedOn");
        builder.Property<DateTime?>("UpdatedOn");

        builder.Navigation(e => e.Addresses).AutoInclude();
        builder.HasQueryFilter(e => EF.Property<DateTime?>(e, "DeletedOn") == null);
    }
}

internal class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", o => o.IsTemporal());

        builder.Property<DateTime?>("DeletedOn");
        builder.Property<DateTime>("CreatedOn");
        builder.Property<DateTime?>("UpdatedOn");

        builder.HasQueryFilter(e => EF.Property<DateTime?>(e, "DeletedOn") == null);
    }
}