using CMS.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace CMS.Repository.Context;

public class CMSDBContext(DbContextOptions<CMSDBContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("CRM_Customers");
        modelBuilder.Entity<Customer>().HasKey(customer => customer.Id).HasName("PK_CRM_Customers");
        modelBuilder.Entity<Customer>(prop =>
        {
            prop.Property(p => p.Id)
                .HasColumnType("int")
                .HasColumnName("Id")
                .UseIdentityColumn();
            prop.Property(p => p.FirstName)
                .HasMaxLength(100)
                .HasColumnName("FirstName");

            prop.Property(p => p.LastName)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100)
                    .HasColumnName("LastName");
            prop.Property(p => p.MiddleName)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100)
                    .HasColumnName("MiddleName");
            prop.Property(p => p.Dob)
                .HasColumnType("DateTime")
                .HasColumnName("Dob");
        });
        modelBuilder.Entity<Customer>()
                    .HasMany<Order>(customer => customer.Orders)
                    .WithOne(order => order.Customer)
                    .HasConstraintName("FK_CRM_Orders_CRM_Customers");

        modelBuilder.Entity<Order>()
                    .ToTable("CRM_Orders",
                            table => table.HasCheckConstraint(
                                                            "CHK_CRM_Orders_OrderStatus",
                                                            $"OrderStatus IN ({(int)OrderStatus.Cancelled}, {(int)OrderStatus.Processing}, {(int)OrderStatus.Shipped}, {(int)OrderStatus.Pending}, {(int)OrderStatus.Delivered})"
                                                           )
                            );
        modelBuilder.Entity<Order>().HasKey(order => order.Id).HasName("PK_CRM_Orders");
        modelBuilder.Entity<Order>(prop =>
        {
            prop.Property(p => p.Id)
                .HasColumnType("int")
                .HasColumnName("Id")
                .UseIdentityColumn();
            prop.Property(p => p.OrderDate)
                .HasColumnType("DateTime")
                .HasColumnName("OrderDate");
            prop.Property(p => p.OrderValue)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("OrderValue");
            prop.Property(p => p.OrderStatus)
                    .HasColumnType("int")
                    .HasColumnName("OrderStatus")
                    .HasDefaultValue(OrderStatus.Pending);

        });
        modelBuilder.Entity<Order>()
                   .HasMany(order => order.OrderDetails)
                   .WithOne(orderDetail => orderDetail.Order)
                   .HasConstraintName("FK_CRM_Orders_OrderDetails");

        modelBuilder.Entity<OrderDetail>().ToTable("CRM_OrderDetails").HasKey(orderDetail => orderDetail.OrderDetailId).HasName("PK_CRM_OrderDetails");
        modelBuilder.Entity<Order>(prop =>
        {
            prop.Property(p => p.Id)
                .HasColumnType("int")
                .HasColumnName("Id")
                .UseIdentityColumn();
            prop.Property(p => p.OrderDate)
                .HasColumnType("DateTime")
                .HasColumnName("OrderDate");
            prop.Property(p => p.OrderValue)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("OrderValue");
            prop.Property(p => p.OrderStatus)
                    .HasColumnType("int")
                    .HasColumnName("OrderStatus")
                    .HasDefaultValue(OrderStatus.Pending);
        });
    }
}
