using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Data;

public class GaaDbContext(DbContextOptions<GaaDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<Log> Logs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Result>(entity =>
        {
            // Primærnøkkel
            entity.HasKey(r => new { r.MemberId, r.RaceId });
            
            // Fremmednøkkler
            entity.HasOne(r => r.Member).WithMany(m => m.Results).HasForeignKey(r => r.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(r => r.Race).WithMany(r => r.Results).HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Datatype TIME for lagring av tid
            entity.Property(r => r.Time).HasColumnType("TIME");
        });
        
        // Primærnøkkel for Member- tabell, unik epost, og auto-increment
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(m => m.MemberId );
            entity.Property(m => m.Gender).HasColumnType("CHAR(1)");
            entity.OwnsOne(m => m.Address, a =>
            {
                a.Property(ad => ad.Street).HasColumnName("Address_Street");
                a.Property(ad => ad.PostalCode).HasColumnName("Address_PostalCode").HasColumnType("SMALLINT");
                a.Property(ad => ad.City).HasColumnName("Address_City");
            });
            entity.HasIndex(m => m.Email).IsUnique();
            entity.Property(m => m.MemberId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(r => new { r.MemberId, RacerId = r.RaceId });
            entity.HasOne(r => r.Member).WithMany(m => m.Registrations).HasForeignKey(r => r.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(r => r.Race).WithMany(r => r.Registrations).HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Primærnøkkel for Race- tabell, og auto-increment
        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(m => m.RaceId);
            entity.Property(m => m.RaceId).ValueGeneratedOnAdd();
        });
    }
}