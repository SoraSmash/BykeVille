using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BykeVille.NewModels;

public partial class DbBikeVilleNewContext : DbContext
{
    public DbBikeVilleNewContext()
    {
    }

    public DbBikeVilleNewContext(DbContextOptions<DbBikeVilleNewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NewAddress> NewAddresses { get; set; }

    public virtual DbSet<NewBuildVersion> NewBuildVersions { get; set; }

    public virtual DbSet<NewCustomer> NewCustomers { get; set; }

    public virtual DbSet<NewCustomerAddress> NewCustomerAddresses { get; set; }

    public virtual DbSet<NewErrorLog> NewErrorLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-LKLMBV5\\SQLEXPRESS;Initial Catalog=DbBikeVilleNew;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<NewAddress>(entity =>
        {
            entity.HasKey(e => e.AddressOldId).HasName("PK_Address_AddressID");

            entity.ToTable("NewAddress", "SalesLT", tb => tb.HasComment("Street address information for customers."));

            entity.HasIndex(e => e.Rowguid, "AK_Address_rowguid").IsUnique();

            entity.HasIndex(e => new { e.AddressLine1, e.AddressLine2, e.City, e.StateProvince, e.PostalCode, e.CountryRegion }, "IX_Address_AddressLine1_AddressLine2_City_StateProvince_PostalCode_CountryRegion");

            entity.HasIndex(e => e.StateProvince, "IX_Address_StateProvince");

            entity.Property(e => e.AddressId)
                .HasComment("Primary key for Address records.")
                .HasColumnName("AddressID")
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(60)
                .HasComment("First street address line.");
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(60)
                .HasComment("Second street address line.");
            entity.Property(e => e.City)
                .HasMaxLength(30)
                .HasComment("Name of the city.");
            entity.Property(e => e.CountryRegion).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(15)
                .HasComment("Postal code for the street address.");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.")
                .HasColumnName("rowguid");
            entity.Property(e => e.StateProvince)
                .HasMaxLength(50)
                .HasComment("Name of state or province.");
            entity.Property(e => e.AddressOldId)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<NewBuildVersion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("NewBuildVersion", tb => tb.HasComment("Current version number of the AdventureWorksLT 2012 sample database. "));

            entity.Property(e => e.DatabaseVersion)
                .HasMaxLength(25)
                .HasComment("Version number of the database in 9.yy.mm.dd.00 format.")
                .HasColumnName("Database Version");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.SystemInformationId)
                .ValueGeneratedOnAdd()
                .HasComment("Primary key for BuildVersion records.")
                .HasColumnName("SystemInformationID");
            entity.Property(e => e.VersionDate)
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<NewCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerOldId).HasName("PK_Customer_CustomerID");

            entity.ToTable("NewCustomer", "SalesLT", tb => tb.HasComment("Customer information."));

            entity.HasIndex(e => e.Rowguid, "AK_Customer_rowguid").IsUnique();

            entity.HasIndex(e => e.EmailAddress, "IX_Customer_EmailAddress");

            entity.Property(e => e.CustomerId)
                .HasComment("Primary key for Customer records.")
                .HasColumnName("CustomerID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(128)
                .HasComment("The customer's organization.");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .HasComment("E-mail address for the person.");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasComment("First name of the person.");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasComment("Last name of the person.");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasComment("Middle name or middle initial of the person.");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.NameStyle).HasComment("0 = The data in FirstName and LastName are stored in western style (first name, last name) order.  1 = Eastern style (last name, first name) order.");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasComment("Password for the e-mail account.");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("Random value concatenated with the password string before the password is hashed.");
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .HasComment("Phone number associated with the person.");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.")
                .HasColumnName("rowguid");
            entity.Property(e => e.SalesPerson)
                .HasMaxLength(256)
                .HasComment("The customer's sales person, an employee of AdventureWorks Cycles.");
            entity.Property(e => e.Suffix)
                .HasMaxLength(10)
                .HasComment("Surname suffix. For example, Sr. or Jr.");
            entity.Property(e => e.Title)
                .HasMaxLength(8)
                .HasComment("A courtesy title. For example, Mr. or Ms.");
            entity.Property(e => e.CustomerOldId);
        });

        modelBuilder.Entity<NewCustomerAddress>(entity =>
        {
            entity.HasKey(e => new { e.CustomerOldId, e.AddressOldId }).HasName("PK_CustomerAddress_CustomerID_AddressID");

            entity.ToTable("NewCustomerAddress", "SalesLT", tb => tb.HasComment("Cross-reference table mapping customers to their address(es)."));

            entity.HasIndex(e => e.Rowguid, "AK_CustomerAddress_rowguid").IsUnique();

            entity.Property(e => e.CustomerId)
                .HasComment("Primary key. Foreign key to Customer.CustomerID.")
                .HasColumnName("CustomerID");
            entity.Property(e => e.AddressId)
                .HasComment("Primary key. Foreign key to Address.AddressID.")
                .HasColumnName("AddressID");
            entity.Property(e => e.AddressType)
                .HasMaxLength(50)
                .HasComment("The kind of Address. One of: Archive, Billing, Home, Main Office, Primary, Shipping");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Date and time the record was last updated.")
                .HasColumnType("datetime");
            entity.Property(e => e.Rowguid)
                .HasDefaultValueSql("(newid())")
                .HasComment("ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.")
                .HasColumnName("rowguid");
            entity.Property(e => e.CustomerOldId);
            entity.Property(e => e.AddressOldId);

            entity.HasOne(d => d.Address).WithMany(p => p.NewCustomerAddresses)
                .HasForeignKey(d => d.AddressOldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddress_Address_AddressID");

            entity.HasOne(d => d.Customer).WithMany(p => p.NewCustomerAddresses)
                .HasForeignKey(d => d.CustomerOldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddress_Customer_CustomerID");
        });

        modelBuilder.Entity<NewErrorLog>(entity =>
        {
            entity.HasKey(e => e.ErrorLogId).HasName("PK_ErrorLog_ErrorLogID");

            entity.ToTable("NewErrorLog", tb => tb.HasComment("Audit table tracking errors in the the AdventureWorks database that are caught by the CATCH block of a TRY...CATCH construct. Data is inserted by stored procedure dbo.uspLogError when it is executed from inside the CATCH block of a TRY...CATCH construct."));

            entity.Property(e => e.ErrorLogId)
                .HasComment("Primary key for ErrorLog records.")
                .HasColumnName("ErrorLogID");
            entity.Property(e => e.ErrorLine).HasComment("The line number at which the error occurred.");
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(4000)
                .HasComment("The message text of the error that occurred.");
            entity.Property(e => e.ErrorNumber).HasComment("The error number of the error that occurred.");
            entity.Property(e => e.ErrorProcedure)
                .HasMaxLength(126)
                .HasComment("The name of the stored procedure or trigger where the error occurred.");
            entity.Property(e => e.ErrorSeverity).HasComment("The severity of the error that occurred.");
            entity.Property(e => e.ErrorState).HasComment("The state number of the error that occurred.");
            entity.Property(e => e.ErrorTime)
                .HasDefaultValueSql("(getdate())")
                .HasComment("The date and time at which the error occurred.")
                .HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(128)
                .HasComment("The user who executed the batch in which the error occurred.");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
