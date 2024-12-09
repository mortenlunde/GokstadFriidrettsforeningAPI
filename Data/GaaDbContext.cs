using GokstadFriidrettsforeningAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace GokstadFriidrettsforeningAPI.Data;

public class GaaDbContext(DbContextOptions<GaaDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Result> Results { get; set; }
    
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
            entity.HasIndex(m => m.Email).IsUnique();
            entity.Property(m => m.MemberId).ValueGeneratedOnAdd();
        });
            
        
        // Primærnøkkel for Race- tabell, og auto-increment
        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(m => m.RaceId);
            entity.Property(m => m.RaceId).ValueGeneratedOnAdd();
        });
    }
}