using Microsoft.EntityFrameworkCore;
using UserIpService.Models;

namespace UserIpService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<IpAddress> IpAddresses { get; set; }
    public DbSet<UserIp> UserIps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserIp>()
            .HasKey(ui => new { ui.UserId, ui.IpAddressId });

        modelBuilder.Entity<UserIp>()
            .HasOne(ui => ui.User)
            .WithMany(u => u.UserIps)
            .HasForeignKey(ui => ui.UserId);

        modelBuilder.Entity<UserIp>()
            .HasOne(ui => ui.IpAddress)
            .WithMany(ip => ip.UserIps)
            .HasForeignKey(ui => ui.IpAddressId);

        base.OnModelCreating(modelBuilder);
    }
}