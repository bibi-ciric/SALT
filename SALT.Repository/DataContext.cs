using Microsoft.EntityFrameworkCore;
using SALT.Domain;

namespace SALT.Repository;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<CakeOriginal> CakeOriginals { get; set; }
    public DbSet<CakeCreated> CakeCreateds { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderCake> OrderCakes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapiranje tabela na mala slova iz baze
        modelBuilder.Entity<Customer>().ToTable("customers");
        modelBuilder.Entity<Ingredient>().ToTable("ingredients");
        modelBuilder.Entity<CakeOriginal>().ToTable("cake_original");
        modelBuilder.Entity<CakeCreated>().ToTable("cake_created");
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<OrderCake>().ToTable("order_cake");

        // Konverzija Enum-a u string za bazu
        modelBuilder.Entity<Ingredient>()
            .Property(i => i.Type)
            .HasConversion<string>();

        // CakeOriginal -> Ingredients (Bottom, Fill, Top, Topping)
        modelBuilder.Entity<CakeOriginal>()
            .HasOne(c => c.BottomLayer)
            .WithMany()
            .HasForeignKey(c => c.BottomLayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeOriginal>()
            .HasOne(c => c.Fill)
            .WithMany()
            .HasForeignKey(c => c.FillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeOriginal>()
            .HasOne(c => c.TopLayer)
            .WithMany()
            .HasForeignKey(c => c.TopLayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeOriginal>()
            .HasOne(c => c.Topping)
            .WithMany()
            .HasForeignKey(c => c.ToppingId)
            .OnDelete(DeleteBehavior.Restrict);

        // CakeCreated -> CakeOriginal i Ingredients
        modelBuilder.Entity<CakeCreated>()
            .HasOne(c => c.CakeOriginal)
            .WithMany()
            .HasForeignKey(c => c.CakeOriginalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeCreated>()
            .HasOne(c => c.BottomLayer)
            .WithMany()
            .HasForeignKey(c => c.BottomLayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeCreated>()
            .HasOne(c => c.Fill)
            .WithMany()
            .HasForeignKey(c => c.FillId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeCreated>()
            .HasOne(c => c.TopLayer)
            .WithMany()
            .HasForeignKey(c => c.TopLayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CakeCreated>()
            .HasOne(c => c.Topping)
            .WithMany()
            .HasForeignKey(c => c.ToppingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Orders -> Customer
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderCake (Spojna tabela) -> Order, CakeOriginal, CakeCreated
        modelBuilder.Entity<OrderCake>()
            .HasOne(oc => oc.Order)
            .WithMany(o => o.OrderCakes)
            .HasForeignKey(oc => oc.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderCake>()
            .HasOne(oc => oc.CakeOriginal)
            .WithMany()
            .HasForeignKey(oc => oc.CakeOriginalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderCake>()
            .HasOne(oc => oc.CakeCreated)
            .WithMany()
            .HasForeignKey(oc => oc.CakeCreatedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}