using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace PharmacyStockManager.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

   

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ExpiryAlert> ExpiryAlerts { get; set; }

    public virtual DbSet<PriceHistory> PriceHistories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }

    public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }

    public virtual DbSet<PurchaseReturnHeader> PurchaseReturnHeaders { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<SalePayment> SalePayments { get; set; }

    public virtual DbSet<SaleReturnDetail> SaleReturnDetails { get; set; }

    public virtual DbSet<SaleReturnHeader> SaleReturnHeaders { get; set; }

    public virtual DbSet<StockLog> StockLogs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connStr = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connStr);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("Categories_pkey");

            entity.HasIndex(e => e.CategoryName, "Categories_CategoryName_key").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Categories_CreatedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CategoryModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Categories_ModifiedBy_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("Customer_pkey");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<ExpiryAlert>(entity =>
        {
            entity.HasKey(e => e.ExpiryAlertId).HasName("ExpiryAlerts_pkey");

            entity.Property(e => e.ExpiryAlertId).HasColumnName("ExpiryAlertID");
            entity.Property(e => e.AlertDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Notified).HasDefaultValue(false);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ExpiryAlerts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ExpiryAlerts_ProductID_fkey");
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.PriceHistoryId).HasName("PriceHistory_pkey");

            entity.ToTable("PriceHistory");

            entity.Property(e => e.PriceHistoryId).HasColumnName("PriceHistoryID");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.NewPurchasePrice).HasPrecision(10, 2);
            entity.Property(e => e.NewSellingPrice).HasPrecision(10, 2);
            entity.Property(e => e.OldPurchasePrice).HasPrecision(10, 2);
            entity.Property(e => e.OldSellingPrice).HasPrecision(10, 2);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.PriceHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PriceHistory_ChangedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.PriceHistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("PriceHistory_ProductID_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("Products_pkey");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.PurchasePrice).HasPrecision(10, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(10, 2);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Products_CategoryID_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Products_ModifiedBy_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Products_SupplierID_fkey");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("Purchases_pkey");

            entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");
            entity.Property(e => e.InvoiceImagePath).HasMaxLength(255);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PurchaseModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Purchases_ModifiedBy_fkey");

            entity.HasOne(d => d.PurchasedByNavigation).WithMany(p => p.PurchasePurchasedByNavigations)
                .HasForeignKey(d => d.PurchasedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Purchases_PurchasedBy_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Purchases_SupplierID_fkey");
        });

        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseDetailId).HasName("PurchaseDetails_pkey");

            entity.Property(e => e.PurchaseDetailId).HasColumnName("PurchaseDetailID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("((\"Quantity\")::numeric * \"UnitPrice\")", true);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("PurchaseDetails_ModifiedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PurchaseDetails_ProductID_fkey");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("PurchaseDetails_PurchaseID_fkey");
        });

        modelBuilder.Entity<PurchaseReturnDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnDetailId).HasName("PurchaseReturnDetail_pkey");

            entity.ToTable("PurchaseReturnDetail");

            entity.Property(e => e.PurchaseReturnDetailId).HasColumnName("PurchaseReturnDetailID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.PurchaseReturnId).HasColumnName("PurchaseReturnID");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("((\"QuantityReturned\")::numeric * \"UnitPrice\")", true);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("PurchaseReturnDetail_ModifiedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PurchaseReturnDetail_ProductID_fkey");

            entity.HasOne(d => d.PurchaseReturn).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.PurchaseReturnId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("PurchaseReturnDetail_PurchaseReturnID_fkey");
        });

        modelBuilder.Entity<PurchaseReturnHeader>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnId).HasName("PurchaseReturnHeader_pkey");

            entity.ToTable("PurchaseReturnHeader");

            entity.Property(e => e.PurchaseReturnId).HasColumnName("PurchaseReturnID");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ReturnDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PurchaseReturnHeaderModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("PurchaseReturnHeader_ModifiedBy_fkey");

            entity.HasOne(d => d.Purchase).WithMany(p => p.PurchaseReturnHeaders)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("PurchaseReturnHeader_PurchaseID_fkey");

            entity.HasOne(d => d.ReturnedByNavigation).WithMany(p => p.PurchaseReturnHeaderReturnedByNavigations)
                .HasForeignKey(d => d.ReturnedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PurchaseReturnHeader_ReturnedBy_fkey");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseReturnHeaders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("PurchaseReturnHeader_SupplierID_fkey");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("Sales_pkey");

            entity.Property(e => e.SaleId).HasColumnName("SaleID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.Customer).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("Sales_CustomerID_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SaleModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Sales_ModifiedBy_fkey");

            entity.HasOne(d => d.SoldByNavigation).WithMany(p => p.SaleSoldByNavigations)
                .HasForeignKey(d => d.SoldBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Sales_SoldBy_fkey");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => e.SaleDetailId).HasName("SaleDetails_pkey");

            entity.Property(e => e.SaleDetailId).HasColumnName("SaleDetailID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SaleId).HasColumnName("SaleID");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("((\"QuantitySold\")::numeric * \"UnitPrice\")", true);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SaleDetails_ModifiedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("SaleDetails_ProductID_fkey");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SaleDetails_SaleID_fkey");
        });

        modelBuilder.Entity<SalePayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("SalePayments_pkey");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.AmountPaid).HasPrecision(12, 2);
            entity.Property(e => e.DueAmount)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0");
            entity.Property(e => e.IsCredit).HasDefaultValue(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.PaymentMode).HasMaxLength(20);
            entity.Property(e => e.SaleId).HasColumnName("SaleID");

            entity.HasOne(d => d.CollectedByNavigation).WithMany(p => p.SalePaymentCollectedByNavigations)
                .HasForeignKey(d => d.CollectedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("SalePayments_CollectedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SalePaymentModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SalePayments_ModifiedBy_fkey");

            entity.HasOne(d => d.Sale).WithMany(p => p.SalePayments)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SalePayments_SaleID_fkey");
        });

        modelBuilder.Entity<SaleReturnDetail>(entity =>
        {
            entity.HasKey(e => e.SaleReturnDetailId).HasName("SaleReturnDetail_pkey");

            entity.ToTable("SaleReturnDetail");

            entity.Property(e => e.SaleReturnDetailId).HasColumnName("SaleReturnDetailID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SaleReturnId).HasColumnName("SaleReturnID");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("((\"QuantityReturned\")::numeric * \"UnitPrice\")", true);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SaleReturnDetails)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SaleReturnDetail_ModifiedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("SaleReturnDetail_ProductID_fkey");

            entity.HasOne(d => d.SaleReturn).WithMany(p => p.SaleReturnDetails)
                .HasForeignKey(d => d.SaleReturnId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SaleReturnDetail_SaleReturnID_fkey");
        });

        modelBuilder.Entity<SaleReturnHeader>(entity =>
        {
            entity.HasKey(e => e.SaleReturnId).HasName("SaleReturnHeader_pkey");

            entity.ToTable("SaleReturnHeader");

            entity.Property(e => e.SaleReturnId).HasColumnName("SaleReturnID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.ReturnDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.SaleId).HasColumnName("SaleID");
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);

            entity.HasOne(d => d.Customer).WithMany(p => p.SaleReturnHeaders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("SaleReturnHeader_CustomerID_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SaleReturnHeaderModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("SaleReturnHeader_ModifiedBy_fkey");

            entity.HasOne(d => d.ReturnedByNavigation).WithMany(p => p.SaleReturnHeaderReturnedByNavigations)
                .HasForeignKey(d => d.ReturnedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("SaleReturnHeader_ReturnedBy_fkey");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleReturnHeaders)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SaleReturnHeader_SaleID_fkey");
        });

        modelBuilder.Entity<StockLog>(entity =>
        {
            entity.HasKey(e => e.StockLogId).HasName("StockLogs_pkey");

            entity.Property(e => e.StockLogId).HasColumnName("StockLogID");
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.ActionType).HasMaxLength(20);
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Reason).HasMaxLength(255);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.StockLogModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("StockLogs_ModifiedBy_fkey");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.StockLogPerformedByNavigations)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("StockLogs_PerformedBy_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.StockLogs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("StockLogs_ProductID_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("Suppliers_pkey");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(20);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.SupplierName).HasMaxLength(100);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Suppliers_ModifiedBy_fkey");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("UserAccounts_pkey");

            entity.HasIndex(e => e.Username, "UserAccounts_Username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.HashedPassword).HasMaxLength(255);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("UserAccounts_CreatedBy_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("UserAccounts_ModifiedBy_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
