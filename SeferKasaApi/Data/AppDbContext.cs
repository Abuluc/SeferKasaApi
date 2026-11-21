using Microsoft.EntityFrameworkCore;
using SeferKasaApi.Entities;
using System;

namespace SeferKasaApi.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>().Property(t => t.AgreedAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Trip>().Property(e => e.CollectedAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Expense>().Property(e => e.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Trip).WithMany(t => t.Expenses).HasForeignKey(e => e.TripId).OnDelete(DeleteBehavior.SetNull);
    }
}


