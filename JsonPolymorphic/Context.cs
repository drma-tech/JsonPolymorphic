using Microsoft.EntityFrameworkCore;

namespace JsonPolymorphic
{
    public class MyDbContext() : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseInMemoryDatabase("TestDB");
        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemConfigurations).Assembly); //get all configurations from the assembly
        }
    }
}