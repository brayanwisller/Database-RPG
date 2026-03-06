using Microsoft.EntityFrameworkCore;
using RpgApi.Models;

namespace RpgApi.Data;
public class AppDbContext : DbContext{
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Item> Items => Set<Item>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseSqlite("Data Source=rpg.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<Item>(e =>{
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).IsRequired().HasMaxLength(100);
            e.HasIndex(i => i.Name).IsUnique();
            e.Property(i => i.Rarity).IsRequired().HasMaxLength(50);
            e.Property(i => i.Preco).IsRequired();
        });
    }
}
