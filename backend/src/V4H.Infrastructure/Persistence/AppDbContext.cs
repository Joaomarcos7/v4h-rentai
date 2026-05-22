using Microsoft.EntityFrameworkCore;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Teleconsultoria> Teleconsultorias => Set<Teleconsultoria>();
    public DbSet<TeleconsultoriaDocument> Documents => Set<TeleconsultoriaDocument>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<Opinion> Opinions => Set<Opinion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
