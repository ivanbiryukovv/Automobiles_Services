using System;
using System.Collections.Generic;
using Automobiles_Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Automobiles_Services.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agreement> Agreements { get; set; }

    public virtual DbSet<Automobile> Automobiles { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<OwnerAutomobile> OwnerAutomobiles { get; set; }

    public virtual DbSet<PassengerCapacity> PassengerCapacities { get; set; }

    public virtual DbSet<ServiceCenter> ServiceCenters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ViewAutomobileOwnerInfo> ViewAutomobileOwnerInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=Automobiles_Services;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agreement>(entity =>
        {
            entity.HasKey(e => e.IdAgreement).HasName("PK__Agreemen__A6A6677B0C392F88");

            entity.ToTable("Agreement");

            entity.Property(e => e.IdAgreement).HasColumnName("ID_agreement");
            entity.Property(e => e.DateOfService).HasColumnName("Date_of_service");
            entity.Property(e => e.GuaranteeUpTo).HasColumnName("Guarantee_up_to");
            entity.Property(e => e.IdOwner).HasColumnName("ID_owner");
            entity.Property(e => e.IdServiceCenter).HasColumnName("ID_service_center");
            entity.Property(e => e.TypeOfService)
                .HasMaxLength(40)
                .HasColumnName("Type_of_service");

            entity.HasOne(d => d.IdOwnerNavigation).WithMany(p => p.Agreements)
                .HasForeignKey(d => d.IdOwner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agreement_Owner");

            entity.HasOne(d => d.IdServiceCenterNavigation).WithMany(p => p.Agreements)
                .HasForeignKey(d => d.IdServiceCenter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agreement_Service_center");
        });

        modelBuilder.Entity<Automobile>(entity =>
        {
            entity.HasKey(e => e.IdAutomobile).HasName("PK__Automobi__C75128E203955E45");

            entity.ToTable("Automobile");

            entity.Property(e => e.IdAutomobile).HasColumnName("ID_automobile");
            entity.Property(e => e.AvailabilityOfAirConditioning)
                .HasMaxLength(3)
                .HasDefaultValue("No")
                .HasColumnName("Availability_of_air_conditioning");
            entity.Property(e => e.BodyType)
                .HasMaxLength(40)
                .HasColumnName("Body_type");
            entity.Property(e => e.Color).HasMaxLength(40);
            entity.Property(e => e.EnginePower)
                .HasMaxLength(20)
                .HasColumnName("Engine_power");
            entity.Property(e => e.IdManufacturer).HasColumnName("ID_manufacturer");
            entity.Property(e => e.Series).HasMaxLength(40);
            entity.Property(e => e.YearOfManufacture).HasColumnName("Year_of_manufacture");

            entity.HasOne(d => d.IdManufacturerNavigation).WithMany(p => p.Automobiles)
                .HasForeignKey(d => d.IdManufacturer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Automobile_Manufacturer");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.IdManufacturer).HasName("PK__Manufact__38AAB9E17DD0A31A");

            entity.ToTable("Manufacturer", tb => tb.HasTrigger("trg_Manufacturer_Delete"));

            entity.Property(e => e.IdManufacturer).HasColumnName("ID_manufacturer");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(40);
            entity.Property(e => e.FoundingDate).HasColumnName("Founding_date");
            entity.Property(e => e.House).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.Street).HasMaxLength(50);
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.IdOwner).HasName("PK__Owner__97C8A80EE3E8CE6C");

            entity.ToTable("Owner");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Owner__7E87EC67AEDB8511").IsUnique();

            entity.Property(e => e.IdOwner).HasColumnName("ID_owner");
            entity.Property(e => e.DriversLicenseNumber)
                .HasMaxLength(6)
                .HasColumnName("Drivers_license_number");
            entity.Property(e => e.DriversLicenseSeries)
                .HasMaxLength(4)
                .HasColumnName("Drivers_license_series");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("First_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(40)
                .HasColumnName("Last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("Phone_number");
            entity.Property(e => e.Surname).HasMaxLength(40);
        });

        modelBuilder.Entity<OwnerAutomobile>(entity =>
        {
            entity.HasKey(e => e.IdOwnerAutomobile).HasName("PK__Owner_au__4377895E5CE1314D");

            entity.ToTable("Owner_automobile");

            entity.Property(e => e.IdOwnerAutomobile).HasColumnName("ID_owner_automobile");
            entity.Property(e => e.IdAutomobile).HasColumnName("ID_automobile");
            entity.Property(e => e.IdOwner).HasColumnName("ID_owner");

            entity.HasOne(d => d.IdAutomobileNavigation).WithMany(p => p.OwnerAutomobiles)
                .HasForeignKey(d => d.IdAutomobile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Owner_automobile_Automobile");

            entity.HasOne(d => d.IdOwnerNavigation).WithMany(p => p.OwnerAutomobiles)
                .HasForeignKey(d => d.IdOwner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Owner_automobile_Owner");
        });

        modelBuilder.Entity<PassengerCapacity>(entity =>
        {
            entity.HasKey(e => e.IdPassengerCapacity).HasName("PK__Passenge__A469134AA1F38E8A");

            entity.ToTable("Passenger_capacity");

            entity.Property(e => e.IdPassengerCapacity).HasColumnName("ID_passenger_capacity");
            entity.Property(e => e.IdAutomobile).HasColumnName("ID_automobile");
            entity.Property(e => e.NumberOfPassengers).HasColumnName("Number_of_passengers");

            entity.HasOne(d => d.IdAutomobileNavigation).WithMany(p => p.PassengerCapacities)
                .HasForeignKey(d => d.IdAutomobile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Passenger_capacity_Automobile");
        });

        modelBuilder.Entity<ServiceCenter>(entity =>
        {
            entity.HasKey(e => e.IdServiceCenter).HasName("PK__Service___60F81B42F6837A26");

            entity.ToTable("Service_center");

            entity.Property(e => e.IdServiceCenter).HasColumnName("ID_service_center");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(40)
                .HasColumnName("Company_name");
            entity.Property(e => e.House).HasMaxLength(10);
            entity.Property(e => e.NameOfTheCenter)
                .HasMaxLength(40)
                .HasColumnName("Name_of_the_center");
            entity.Property(e => e.NumberOfSeats).HasColumnName("Number_of_seats");
            entity.Property(e => e.Street).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__D7B4671E24881F86");

            entity.ToTable("User");

            entity.Property(e => e.IdUser).HasColumnName("ID_user");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewAutomobileOwnerInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_AutomobileOwnerInfo");

            entity.Property(e => e.Color).HasMaxLength(40);
            entity.Property(e => e.EnginePower)
                .HasMaxLength(20)
                .HasColumnName("Engine_power");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("First_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(40)
                .HasColumnName("Last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("Phone_number");
            entity.Property(e => e.Surname).HasMaxLength(40);
            entity.Property(e => e.YearOfManufacture).HasColumnName("Year_of_manufacture");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
